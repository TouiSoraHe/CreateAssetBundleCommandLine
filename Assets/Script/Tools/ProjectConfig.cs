using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
# if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class ProjectConfig {
    [SerializeField] private String AssetBundleBuildTarget;


    private static ProjectConfig instance;

#if UNITY_EDITOR
    public BuildTarget m_AssetBundleBuildTarget
    {
        get
        {
            return EnumTools.GetEnum<BuildTarget>(AssetBundleBuildTarget);
        }
        set
        {
            AssetBundleBuildTarget = EnumTools.GetString(value);
        }
    }
#endif

    public static ProjectConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = JsonTools.ResolutionJsonFromFile<ProjectConfig>(GlobalConstants.ProjectConfigFilePath);
            }
            return instance;
        }
    }
}
