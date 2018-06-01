using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetsSetting {

    /// <summary>
    /// 清空Resources目录
    /// </summary>
    public static void ClearResourcesDir()
    {
        FileTools.DeleteDirectory(GlobalConstants.AbsoluteResourcesPath);
        FileTools.CreateDirectory(GlobalConstants.AbsoluteResourcesPath);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 将srcPath上的文件复制到Resources目录下,并且导入资源
    /// </summary>
    /// <param name="srcPath">需要导入的资源的绝对路径</param>
    /// <returns>导入后的资源的路径(相对路径,基于Assets目录)</returns>
    public static string ImportAsset(string srcPath)
    {
        if (string.IsNullOrEmpty(srcPath))
        {
            throw new System.Exception("资源文件路径不能为空");
        }
        if (!FileTools.FileExists(srcPath))
        {
            throw new System.Exception("资源文件不存在");
        }
        FileTools.CopyFile(srcPath, GlobalConstants.AbsoluteResourcesPath + "/" + FileTools.GetFileName(srcPath));
        AssetDatabase.Refresh();
        srcPath = GlobalConstants.RelativelyResourcesPath + "/" + FileTools.GetFileName(srcPath);
        AssetDatabase.ImportAsset(srcPath);
        return srcPath;
    }

    /// <summary>
    /// 导入设置,导入时对资源进行各种修改和设置
    /// </summary>
    public static void ImporterSet(ConfigJson configJson,string srcPath)
    {
        //导入设置
        if (configJson.m_ModelConfig != null)
        {
            SetModelConfig(configJson.m_ModelConfig, srcPath);
        }
        if (configJson.m_TextureConfig != null)
        {
            SetTextureConfig(configJson.m_TextureConfig, srcPath);
        }

        AssetDatabase.ImportAsset(srcPath);

        //GameObject设置,在该注释以后部分再也不能调用AssetDatabase.ImportAsset,否则以下设置将无效
        GameObject go = Resources.Load<GameObject>(FileTools.GetFileNameWithoutExtension(srcPath));
        GameobjectSet(configJson, go);
    }

    private static void GameobjectSet(ConfigJson configJson,GameObject go)
    {
        if (go != null)
        {
            //关闭老动画自动播放
            Animation animation = go.GetComponent<Animation>();
            if (animation != null)
            {
                animation.playAutomatically = configJson.m_ModelConfig.m_PlayAutomatically;
            }
            //设置物体的transform信息
            SetTransform(configJson, go);
        }
    }

    private static void SetTransform(ConfigJson configJson, GameObject go)
    {
        Vector3 pos = go.transform.position;
        go.transform.position = Vector3.zero;
        PerspectiveMode perspectiveMode = configJson.m_ModelConfig.m_PerspectiveMode;
        switch (perspectiveMode.m_CaululateType)
        {
            case CalculateModelNativePos.CalculateType.FullScreen:
            case CalculateModelNativePos.CalculateType.Real:
                pos = CalculateModelNativePos.Caululate(go.transform, perspectiveMode.m_CalculateCamera, perspectiveMode.m_CaululateType);
                break;
            case CalculateModelNativePos.CalculateType.Node:
                Transform node = go.transform.Find(perspectiveMode.m_CalculateChildPath);
                if (node == null)
                {
                    node = go.transform;
                }
                pos = CalculateModelNativePos.Caululate(node, perspectiveMode.m_CalculateCamera, perspectiveMode.m_CaululateType);
                break;
            default:
                break;
        }
        go.transform.position = pos;
    }

    /// <summary>
    /// 设置资源的ModelImporterAnimationType
    /// </summary>
    private static void SetModelConfig(ModelConfig modelConfig, string srcPath)
    {
        ModelImporter import = AssetImporter.GetAtPath(srcPath) as ModelImporter;
        if (import != null)
        {
            //自动将新动画转为老动画
            if (modelConfig.m_GenericToLegacy)
            {
                if (import.animationType == ModelImporterAnimationType.Generic)
                {
                    import.animationType = ModelImporterAnimationType.Legacy;
                }
            }
            //设置模型的动画类型
            if (modelConfig.m_ModelAnimationType != EnumTools.GetException<ModelImporterAnimationType>())
            {
                import.animationType = modelConfig.m_ModelAnimationType;
            }
            //设置模型的材质模板
            if (modelConfig.m_MaterialTypes != null && modelConfig.m_MaterialTypes.Length > 0)
            {
                foreach (var item in modelConfig.m_MaterialTypes)
                {
                    Material material = Resources.Load("Materials/" + item.m_MaterialName) as Material;
                    if (material != null)
                    {
                        MaterialTemplateTools.SetMaterialShaderProperty(material, item.m_TemplateName);
                    }
                }
            }
            //判断模型上面的材质是否包含法线贴图,如果有法线贴图则找到贴图,并将贴图的类型改为NormalMap
            SetNormalMap();
            //待扩展
        }
    }

    /// <summary>
    /// 设置texture的信息
    /// </summary>
    /// <param name="size"></param>
    private static void SetTextureConfig(TextureConfig textureConfig, string srcPath)
    {
        TextureImporter import = AssetImporter.GetAtPath(srcPath) as TextureImporter;
        if (import != null)
        {
            //设置Texture的最大大小
            if (textureConfig.m_TextureSize != -1)
            {
                import.maxTextureSize = textureConfig.m_TextureSize;
            }
            //设置Texture的类型
            if (textureConfig.m_TextureType != EnumTools.GetException<TextureImporterType>())
            {
                import.textureType = textureConfig.m_TextureType;
            }
            //设置Texture的WrapMode
            if (textureConfig.m_WrapMode != EnumTools.GetException<TextureWrapMode>())
            {
                import.wrapMode = textureConfig.m_WrapMode;
            }
            //设置Texture的FilterMode
            if (textureConfig.m_FilterMode != EnumTools.GetException<FilterMode>())
            {
                import.filterMode = textureConfig.m_FilterMode;
            }
            //待扩展
        }
    }

    /// <summary>
    /// 判断模型上面的材质是否包含法线贴图,如果有法线贴图则找到贴图,并将贴图的类型改为NormalMap
    /// </summary>
    private static void SetNormalMap()
    {
        string[] materials = FileTools.GetFileSystemEntries(GlobalConstants.AbsoluteResourcesPath + "/Materials");
        string fbmDirPath = null;
        if (materials != null && materials.Length > 0)
        {
            foreach (var materialPath in materials)
            {
                string materialName = FileTools.GetFileNameWithoutExtension(materialPath);
                Material material = Resources.Load<Material>("Materials/" + materialName);
                if (material != null)
                {
                    Texture normalMapTexture = material.GetTexture("_BumpMap");
                    if (normalMapTexture != null)
                    {
                        if (string.IsNullOrEmpty(fbmDirPath))
                        {
                            foreach (var resourcesChildFilePath in FileTools.GetFileSystemEntries(GlobalConstants.AbsoluteResourcesPath))
                            {
                                if ("fbm".Equals(FileTools.GetExtension(resourcesChildFilePath)))
                                {
                                    fbmDirPath = resourcesChildFilePath;
                                }
                            }
                        }
                        if(!string.IsNullOrEmpty(fbmDirPath))
                        {
                            foreach (var textureFilePath in FileTools.GetFileSystemEntries(fbmDirPath))
                            {
                                if (normalMapTexture.name.Equals(FileTools.GetFileNameWithoutExtension(textureFilePath)))
                                {
                                    string textureResourcesPath = GlobalConstants.RelativelyResourcesPath + "/" + FileTools.GetFileName(FileTools.GetDirectoryName(textureFilePath));
                                    textureResourcesPath += "/" + FileTools.GetFileName(textureFilePath);
                                    TextureImporter import = AssetImporter.GetAtPath(textureResourcesPath) as TextureImporter;
                                    if (import != null)
                                    {
                                        import.textureType = TextureImporterType.NormalMap;
                                        Debug.Log(textureResourcesPath + "的TextureImporterType被设置为" + TextureImporterType.NormalMap);
                                        material.EnableKeyword("_NORMALMAP");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 将打包好的临时文件移动到输出目录
    /// </summary>
    public static void MoveOutAsset(string tempPath,string outPath)
    {
        if (!FileTools.FileExists(tempPath))
        {
            throw new System.Exception("抱歉,打包失败,详细信息请查看日志信息");
        }
        Debug.Log("打包结束,开始将资源移动到指定路径,outputpath:" + outPath);
        if (!FileTools.DirectoryExists(FileTools.GetDirectoryName(outPath)))
        {
            FileTools.CreateDirectory(FileTools.GetDirectoryName(outPath));
        }
        FileTools.MoveFile(tempPath, outPath);
    }
}
