using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConstants {
    public const string Delimiter = "!@#$%^&*()";
    public static readonly string AbsoluteResourcesPath = Application.dataPath + "/Resources";
    public const string RelativelyResourcesPath = "Assets/Resources";
    public static readonly string TempPath = AbsoluteResourcesPath + "/Temp";
    public static readonly string ProjectConfigFilePath = FileTools.GetDirectoryName(FileTools.GetDirectoryName(Application.dataPath)) + "/project.config";//项目同级目录
    public static readonly string LogFilePath = FileTools.GetDirectoryName(Application.dataPath) + "/log.txt";
}
