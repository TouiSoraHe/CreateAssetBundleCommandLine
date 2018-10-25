using System;
# if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class ConfigJson {
    [SerializeField] private ModelConfig ModelConfig;//模型配置
    [SerializeField] private TextureConfig TextureConfig;//贴图配置

    public ModelConfig m_ModelConfig
    {
        get
        {
            if (ModelConfig == null)
            {
                ModelConfig = new ModelConfig();
            }
            return ModelConfig;
        }

        set
        {
            ModelConfig = value;
        }
    }

    public TextureConfig m_TextureConfig
    {
        get
        {
            if (TextureConfig == null)
            {
                TextureConfig = new TextureConfig();
            }
            return TextureConfig;
        }

        set
        {
            TextureConfig = value;
        }
    }
}

[Serializable]
public class ModelConfig
{
    /// <summary>
    /// 设置AnimationType
    /// </summary>
    [SerializeField] private string ModelAnimationType;//模型动画类型
    [SerializeField] private MaterialType[] MaterialTypes;//模型材质模板
    [SerializeField] private bool GenericToLegacy = true;//自动将老动画转为新动画,如果设置了ModelAnimationType,ModelAnimationType的设置将会覆盖该项
    [SerializeField] private bool PlayAutomatically = false;//是否自动播放动画,false代表不自动播放
    [SerializeField] private PerspectiveMode PerspectiveMode;//视角模式

# if UNITY_EDITOR
    public ModelImporterAnimationType m_ModelAnimationType
    {
        get
        {
            return EnumTools.GetEnum<ModelImporterAnimationType>(ModelAnimationType);
        }
        set
        {
            ModelAnimationType = EnumTools.GetString(value);
        }
    }
#endif

    public MaterialType[] m_MaterialTypes
    {
        get
        {
            if (MaterialTypes == null)
            {
                MaterialTypes = new MaterialType[0];
            }
            return MaterialTypes;
        }

        set
        {
            MaterialTypes = value;
        }
    }

    public bool m_GenericToLegacy
    {
        get
        {
            return GenericToLegacy;
        }

        set
        {
            GenericToLegacy = value;
        }
    }

    public bool m_PlayAutomatically
    {
        get
        {
            return PlayAutomatically;
        }

        set
        {
            PlayAutomatically = value;
        }
    }

    public PerspectiveMode m_PerspectiveMode
    {
        get
        {
            if (PerspectiveMode == null)
            {
                PerspectiveMode = new PerspectiveMode();
            }
            return PerspectiveMode;
        }

        set
        {
            PerspectiveMode = value;
        }
    }
}

[Serializable]
public class PerspectiveMode
{
    [SerializeField] private string CalculateType = "Node";
    [SerializeField] private string CalculateChildPath = "";//默认聚焦到根节点
    [SerializeField] private float CameraX = 0f;
    [SerializeField] private float CameraY = 0f;
    [SerializeField] private float CameraZ = -10f;
    private Camera camera = null;

    public CalculateModelNativePos.CalculateType m_CaululateType
    {
        get
        {
            return EnumTools.GetEnum<CalculateModelNativePos.CalculateType>(CalculateType);
        }

        set
        {
            CalculateType = EnumTools.GetString(value);
        }
    }

    public string m_CalculateChildPath
    {
        get
        {
            return CalculateChildPath;
        }

        set
        {
            CalculateChildPath = value;
        }
    }

    public Camera m_CalculateCamera
    {
        get
        {
            if (camera == null)
            {
                camera = new GameObject().AddComponent<Camera>();
                camera.transform.position = new Vector3(CameraX, CameraY, CameraZ);
            }
            return camera;
        }
    }
}

[Serializable]
public class MaterialType
{
    [SerializeField] private string MaterialName;
    [SerializeField] private string TemplateName;

    public string m_MaterialName
    {
        get
        {
            return MaterialName;
        }

        set
        {
            MaterialName = value;
        }
    }

    public string m_TemplateName
    {
        get
        {
            return TemplateName;
        }

        set
        {
            TemplateName = value;
        }
    }
}

[Serializable]
public class TextureConfig
{
    [SerializeField] private string TextureType;
    [SerializeField] private int MaxTextureSize = -1;
    [SerializeField] private string WrapMode;
    [SerializeField] private string FilterMode;

#if UNITY_EDITOR
    public TextureImporterType m_TextureType
    {
        get
        {
            return EnumTools.GetEnum<TextureImporterType>(TextureType);
        }

        set
        {
            TextureType = EnumTools.GetString(value);
        }
    }
#endif

    public int m_TextureSize
    {
        get
        {
            return MaxTextureSize;
        }

        set
        {
            MaxTextureSize = value;
        }
    }

    public TextureWrapMode m_WrapMode
    {
        get
        {
            return EnumTools.GetEnum<TextureWrapMode>(WrapMode);
        }

        set
        {
            WrapMode = EnumTools.GetString(value);
        }
    }

    public FilterMode m_FilterMode
    {
        get
        {
            return EnumTools.GetEnum<FilterMode>(FilterMode);
        }

        set
        {
            FilterMode = EnumTools.GetString(value);
        }
    }
}
