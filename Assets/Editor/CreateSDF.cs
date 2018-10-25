using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateSDF
{
    private const string Delimiter = "!@#$%^&*()";
    private static string srcPath;
    private static string outPath;
    private static ConfigJson configJson;
    private static string[] commandLineArgs = null;

    public static void MainMethod()
    {
        try
        {
            commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length < 11)
            {
                throw new System.Exception("srcPath不能为空");
            }
            if (commandLineArgs.Length < 12)
            {
                throw new System.Exception("outpath不能为空");
            }
            srcPath = commandLineArgs[10];
            outPath = commandLineArgs[11];
            //获取扩展设置
            string configInfoJson = "";
            if (commandLineArgs.Length > 12)
            {
                configInfoJson = commandLineArgs[12];
            }
            configJson = JsonTools.ResolutionJsonFromString<ConfigJson>(configInfoJson);
            StartBuildSDF();
            LogTools.Info("打包结束,未发现异常.srcPath:"+ commandLineArgs [10]+ " outPath:" + commandLineArgs[11]);
        }
        catch (System.Exception e)
        {
            if (FileTools.FileExists(srcPath))
            {
                LogTools.Info("发生异常的文件存在,文件路径为:" + srcPath);
                if (!FileTools.DirectoryExists(GlobalConstants.ExceptionFBXFolder))
                {
                    FileTools.CreateDirectory(GlobalConstants.ExceptionFBXFolder);
                }
                FileTools.CopyFile(srcPath, GlobalConstants.ExceptionFBXFolder + "/" + FileTools.GetFileName(srcPath) + "(" + DateTime.Now.ToString() + ")");
            }
            else
            {
                LogTools.Info("发生异常的文件存在,文件路径为:" + srcPath);
            }
            PrintError(e.Message);
            LogTools.Error(e.Message + "\n" + GetDetailsInfo(), e);
        }
    }

    private static string GetDetailsInfo()
    {
        string ret = "当前详细状态:\n";
        string temp = "";
        if (commandLineArgs != null)
        {
            foreach (var item in commandLineArgs)
            {
                temp += item + " ";
            }
            ret += "命令行参数:" + temp + "\n";
            if (commandLineArgs.Length > 10)
            {
                bool fileExits = false;
                try
                {
                    fileExits = FileTools.FileExists(commandLineArgs[10]);
                }
                catch (System.Exception e) { }
                ret += "输入文件路径:" + commandLineArgs[10] + " 文件是否存在:" + fileExits + "\n";
                string importFilePath = GlobalConstants.AbsoluteResourcesPath + "/" + FileTools.GetFileName(commandLineArgs[10]);
                ret += "导入文件路径:" + importFilePath + " 文件是否存在:" + FileTools.FileExists(importFilePath) + "\n";
                string abFilePath = GlobalConstants.TempPath + "/" + FileTools.GetFileName(commandLineArgs[10]).ToLower() + ".ab";
                ret += "打包文件路径:" + abFilePath + " 文件是否存在:" + FileTools.FileExists(abFilePath) + "\n";
            }
        }
        else
        {
            ret += "命令行参数:命令行参数为空\n";
        }
        return ret;

    }

    public static void StartBuildSDF()
    {
        AssetsSetting.ClearResourcesDir();                                  //清空Resources目录
        srcPath = AssetsSetting.ImportAsset(srcPath);                       //将srcPath上的文件复制到Resources目录下,并且导入资源
        AssetsSetting.ImporterSet(configJson, srcPath);           //对资源进行各种参数修改和设置再次导入
        ExportSDFJsonDataWithRootPath(GlobalConstants.TempPath, FileTools.GetFileName(srcPath).ToLower());
        string sdfPath = GlobalConstants.TempPath + "/" + FileTools.GetFileName(srcPath).ToLower() + ".sdf";
        AssetsSetting.MoveOutAsset(sdfPath, outPath);                       //将打包好的文件移动到输出目录
        AssetsSetting.ClearResourcesDir();                                  //清空Resources目录
    }

    public static void ExportSDFJsonDataWithRootPath(string path, string sceneName)
    {
        SDF sdf = null;
        GameObject[] goList = null;
        GameObject gameObject = Resources.Load<GameObject>(FileTools.GetFileNameWithoutExtension(srcPath));
        if (gameObject != null)
        {
            goList = new GameObject[] { gameObject };
        }
        if (goList == null || goList.Length < 1)
        {
            throw new Exception("模型文件有误");
        }
        sdf = new SDF();
        sdf.paramsObj = new ParamsData();
        sdf.paramsObj.initView.viewId = "default";
        sdf.paramsObj.initView.viewName = "default";
        List<ModelInfo> data = new List<ModelInfo>();
        for (int i = 0; i < goList.Length; i++)
        {
            //遍历模型
            Transform currentGo = goList[i].transform;
            Animation animation = currentGo.GetComponent<Animation>();
            if (animation != null)
            {
                Debug.Log("当前动画组件有动画:" + animation.GetClipCount());
                AnimationClip[] aniList = AnimationUtility.GetAnimationClips(currentGo.gameObject);
                for (int x = 0; x < aniList.Length; x++)
                {
                    Debug.Log("动画片段名称:" + aniList[x].name);
                    sdf.paramsObj.innerAnimations.Add(new KeyAnimation(currentGo.name, aniList[x].name));
                }
            }

            ModelInfo modelInfo = new ModelInfo();
            modelInfo.spaceState[0] = ConvertIntNumber(currentGo.transform.localPosition.x);
            modelInfo.spaceState[1] = ConvertIntNumber(currentGo.transform.localPosition.y);
            modelInfo.spaceState[2] = ConvertIntNumber(currentGo.transform.localPosition.z);

            modelInfo.spaceState[3] = ConvertIntNumber(currentGo.transform.eulerAngles.x);
            modelInfo.spaceState[4] = ConvertIntNumber(currentGo.transform.eulerAngles.y);
            modelInfo.spaceState[5] = ConvertIntNumber(currentGo.transform.eulerAngles.z);

            modelInfo.spaceState[6] = ConvertIntNumber(currentGo.transform.localScale.x);
            modelInfo.spaceState[7] = ConvertIntNumber(currentGo.transform.localScale.y);
            modelInfo.spaceState[8] = ConvertIntNumber(currentGo.transform.localScale.z);

            modelInfo.modelId = currentGo.name;
            modelInfo.webLabel = currentGo.name;
            Debug.Log("添加场景模型:" + goList[i].name);
            modelInfo.subNodes = new List<NodeInfo>();
            NodeInfo root = new NodeInfo();
            root.nodeId = currentGo.name;
            root.webLabel = currentGo.name;
            root.subNodes = new List<NodeInfo>();
            modelInfo.subNodes.Add(root);
            Dictionary<string, int> nodeDic = new Dictionary<string, int>();
            for (int m = 0; m < currentGo.childCount; m++)
            {
                string nodeName = currentGo.GetChild(m).name;
                if (nodeDic.ContainsKey(nodeName))
                {
                    nodeDic[nodeName] = nodeDic[nodeName] + 1;
                    currentGo.GetChild(m).name = currentGo.GetChild(m).name + "_" + nodeDic[nodeName];
                    Debug.LogFormat("{0}的子节点中，出现重复的节点,添加角标后的名字为:{1}", currentGo.name, currentGo.GetChild(m).name);
                }
                else
                {
                    nodeDic.Add(nodeName, 0);
                }
            }
            for (int m = 0; m < currentGo.childCount; m++)
            {
                //遍历第一层级
                NodeInfo ni = new NodeInfo();
                ni.nodeId = currentGo.GetChild(m).name;
                ni.webLabel = ni.nodeId;
                Transform curGo = currentGo.GetChild(m);
                ni.subNodes = new List<NodeInfo>();
                Dictionary<string, int> nodeDicTwo = new Dictionary<string, int>();
                for (int k = 0; k < curGo.childCount; k++)
                {
                    string nodeName = curGo.GetChild(k).name;
                    if (nodeDicTwo.ContainsKey(nodeName))
                    {
                        nodeDicTwo[nodeName] = nodeDicTwo[nodeName] + 1;
                        curGo.GetChild(k).name = curGo.GetChild(k).name + "_" + nodeDicTwo[nodeName];
                        Debug.LogFormat("{0}的子节点中，出现重复的节点,添加角标后的名字为:{1}", curGo.name, curGo.GetChild(k).name);
                    }
                    else
                    {
                        nodeDicTwo.Add(nodeName, 0);
                    }
                }
                for (int n = 0; n < curGo.childCount; n++)
                {
                    //遍历第二层级
                    NodeInfo subNode = new NodeInfo();
                    subNode.nodeId = curGo.GetChild(n).name;
                    subNode.webLabel = subNode.nodeId;
                    ni.subNodes.Add(subNode);
                }
                modelInfo.subNodes[0].subNodes.Add(ni);
            }

            data.Add(modelInfo);
            UpdatePrefab(currentGo.gameObject);
        }
        sdf.models = data;

        string allModelJsonData = JsonUtility.ToJson(sdf);
        allModelJsonData = allModelJsonData.Replace("paramsObj", "params");
        Debug.Log("导出场景SDF文件:" + allModelJsonData);
        string fileName = path + "/" + sceneName + ".sdf";
        FileTools.WriteFileUtf8Create(fileName, allModelJsonData);
        Debug.Log(fileName + "场景SDF文件创建完毕！");
        AssetDatabase.Refresh();
    }

    private static float ConvertIntNumber(float oldVal, int number = 4)
    {
        int divisor = (int)Mathf.Pow(10, number);
        float val = Mathf.Round(oldVal * divisor) / divisor;
        //Debug.Log("转换后:" + val);
        //通过UnityJson转换后float的值会改变，这里序列化后面会换个其它的库
        return oldVal;
    }


    private static void UpdatePrefab(GameObject go)
    {
        //判断选择的物体，是否为预设  
        if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
        {
            UnityEngine.Object parentObject = PrefabUtility.GetPrefabParent(go);
            //替换预设  
            PrefabUtility.ReplacePrefab(go, parentObject, ReplacePrefabOptions.ConnectToPrefab);
            Debug.Log("解决子节点重名问题后，更新预设");
            //刷新  
            AssetDatabase.Refresh();
        }
    }

    private static void MoveAsset()
    {
        string sdfPath = GlobalConstants.TempPath + "/" + FileTools.GetFileName(srcPath).ToLower() + ".sdf";
        if (!FileTools.FileExists(sdfPath))
        {
            throw new Exception("抱歉,创建sdf文件失败,详细信息请查看日志信息");
        }
        Debug.Log("打包结束,开始将资源移动到指定路径,outputpath:" + outPath);
        if (!FileTools.DirectoryExists(FileTools.GetDirectoryName(outPath)))
        {
            FileTools.CreateDirectory(FileTools.GetDirectoryName(outPath));
        }
        FileTools.MoveFile(sdfPath, outPath);
    }


    //场景描述文件
    [Serializable]
    public class SDF
    {
        public ParamsData paramsObj;
        public List<ModelInfo> models;
        public SDF(List<ModelInfo> modelsP, ParamsData paramsObjP)
        {
            paramsObj = paramsObjP;
            models = modelsP;
        }
        public SDF()
        { }
    }


    [Serializable]
    public class SceneView
    {
        public string viewId;//视角ID
        public string viewName;//视角名称
        public float[] cameraParams = new float[] { 0, 0, 0, 0, -10f, 0, 0, 0, 0 };
        public SceneView()
        {

        }
    }
    [Serializable]
    public class KeyAnimation
    {
        public string animationIdentifier;
        public string animationName;
        public string modelName;
        public KeyAnimation(string modelNameP, string animationNameP)
        {
            modelName = modelNameP;
            animationIdentifier = animationNameP;
            animationName = animationNameP;
        }
    }
    [Serializable]
    public class ParamsData
    {
        public SceneView initView;
        public List<KeyAnimation> innerAnimations;
        public ParamsData()
        {
            initView = new SceneView();
            innerAnimations = new List<KeyAnimation>();
        }

    }
    [Serializable]
    public class ModelInfo
    {
        public string modelId;
        public string webLabel;
        public float[] spaceState = new float[9];
        public List<NodeInfo> subNodes;
    }
    [Serializable]
    public class NodeInfo
    {
        public string nodeId;
        public string webLabel;
        public List<NodeInfo> subNodes;
    }

    private static void PrintError(string error)
    {
        Debug.Log(Delimiter + error + Delimiter);
    }
}