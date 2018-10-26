using System;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundle : Editor {

    private static string srcPath;
    private static string outPath;
    private static ConfigJson configJson;
    private static BuildTarget buildTarget = BuildTarget.StandaloneWindows;
    private static string[] commandLineArgs = null;

    public static void MainMethod()
    {
        try
        {
            commandLineArgs = System.Environment.GetCommandLineArgs();
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
            //设置打包平台
            if (ProjectConfig.Instance.m_AssetBundleBuildTarget != EnumTools.GetException<BuildTarget>())
            {
                buildTarget = ProjectConfig.Instance.m_AssetBundleBuildTarget;
            }
            StartBuildBundle();
            LogTools.Info("打包结束,未发现异常.srcPath:" + commandLineArgs[10] + " outPath:" + commandLineArgs[11] + " configInfoJson:" + (commandLineArgs.Length > 12 ? commandLineArgs[12] : ""));
        }
        catch (System.Exception e)
        {
            PrintExcepitonLog(e);
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
                ret += "输入文件路径:" + commandLineArgs[10] + " 文件是否存在:" + fileExits+"\n";
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

    private static void PrintExcepitonLog(Exception e)
    {
        if (e.GetType() == typeof(InvalidOperationException))
        {
            LogTools.PrintError("打包失败,文件已损坏");

        }
        else
        {
            LogTools.PrintError(e.GetType().Name + e.Message);
        }
        LogTools.Error(e.Message + "\n" + GetDetailsInfo(), e);
        try
        {
            String exceptionFilePath = GlobalConstants.AbsoluteResourcesPath + "/" + FileTools.GetFileName(srcPath);
            if (FileTools.FileExists(exceptionFilePath))
            {
                LogTools.Info("发生异常的文件存在,文件路径为:" + exceptionFilePath);
                if (!FileTools.DirectoryExists(GlobalConstants.ExceptionFBXFolder))
                {
                    FileTools.CreateDirectory(GlobalConstants.ExceptionFBXFolder);
                }
                String destFilePath = GlobalConstants.ExceptionFBXFolder + "/" + FileTools.GetFileName(exceptionFilePath)+"("+ DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + ")";
                FileTools.CopyFile(exceptionFilePath, destFilePath);
            }
            else
            {
                LogTools.Info("发生异常的文件不存在,文件路径为");
            }
        }
        catch (Exception ee)
        {
            Debug.Log(ee);
            LogTools.PrintError(ee.GetType().Name + ee.Message);
            LogTools.Error(ee.Message + "\n" + GetDetailsInfo(), ee);
        }
    }

    /// <summary>
    /// 该方法用于测试使用
    /// </summary>
    [MenuItem("Test/Test")]
    public static void Test()
    {
        srcPath = "D:/fbx/test.FBX";
        outPath = "D:/fbx/test.ab";
        //获取扩展设置
        string configInfoJson = "";
        configJson = JsonTools.ResolutionJsonFromString<ConfigJson>(configInfoJson);
        try
        {
            StartBuildBundle();
        }
        catch (System.Exception e)
        {
            PrintExcepitonLog(e);
        }
    }

    public static void StartBuildBundle()
    {
        AssetsSetting.ClearResourcesDir();                                  //清空Resources目录
        srcPath = AssetsSetting.ImportAsset(srcPath);                       //将srcPath上的文件复制到Resources目录下,并且导入资源
        AssetsSetting.ImporterSet(configJson, srcPath);           //对资源进行各种参数修改和设置再次导入
        ClearAssetBundlesName();                                            //清空AssetBundlesName
        PerpareToBuild(srcPath);                                            //设置打包资源的assetBundleName
        StartToBuild();                                                     //开始打包
        string assetBundlePath = GlobalConstants.TempPath + "/" + FileTools.GetFileName(srcPath).ToLower() + ".ab";
        AssetsSetting.MoveOutAsset(assetBundlePath,outPath);                //将打包好的文件移动到输出目录
        AssetsSetting.ClearResourcesDir();                                  //清空Resources目录
    }



    /// <summary>
    /// 清空AssetBundlesName
    /// </summary>
    private static void ClearAssetBundlesName()
    {
        foreach (var item in AssetDatabase.GetAllAssetBundleNames())
        {
            AssetDatabase.RemoveAssetBundleName(item, true);
        }
    }

    /// <summary>
    /// 设置打包资源的assetBundleName
    /// </summary>
    /// <param name="srcPath"></param>
    private static void PerpareToBuild(string srcPath)
    {
        string AssetName = FileTools.GetFileName(srcPath);
        Debug.Log("资源文件路径:" + srcPath);
        AssetImporter assetImporter = AssetImporter.GetAtPath(srcPath);
        if (assetImporter == null)
        {
            throw new System.Exception("在" + srcPath + "上获取AssetImporter失败");
        }
        assetImporter.assetBundleName = AssetName;
        assetImporter.assetBundleVariant = "ab";
    }

    /// <summary>
    /// 开始打包
    /// </summary>
    private static void StartToBuild()
    {
        if (!FileTools.DirectoryExists(GlobalConstants.TempPath))
        {
            FileTools.CreateDirectory(GlobalConstants.TempPath);
        }
        Debug.Log("当前打包平台:"+ buildTarget.ToString());
        BuildPipeline.BuildAssetBundles(GlobalConstants.TempPath, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);
    }

}
