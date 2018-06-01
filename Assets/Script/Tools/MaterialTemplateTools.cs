using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class MaterialTemplateTools {
    public static void SetMaterialShaderProperty(Material material,string templateName)
    {
        if (material != null)
        {
            string templateJson = FileTools.ReadFileUTf8(Application.streamingAssetsPath + "/" + templateName+".json");
            if (!string.IsNullOrEmpty(templateJson))
            {
                StandardMaterialTemplate standardMaterialTemplate = JsonTools.ResolutionJsonFromString<StandardMaterialTemplate>(templateJson);
                //设置RenderingMode
                if (!string.IsNullOrEmpty(standardMaterialTemplate.RenderingMode))
                {
                    SetMaterialRenderingMode(material, standardMaterialTemplate.RenderingMode);
                }
                //设置_Color
                if (!string.IsNullOrEmpty(standardMaterialTemplate.MainMapsColor))
                {
                    SetColor(material, "_Color", standardMaterialTemplate.MainMapsColor);
                }
                //设置Alpha Cutoff
                if (!string.IsNullOrEmpty(standardMaterialTemplate.AlphaCutoff))
                {
                    SetFloatRange(material, "_Cutoff", standardMaterialTemplate.AlphaCutoff, 0f, 1f);
                }
                //设置Metallic
                if (!string.IsNullOrEmpty(standardMaterialTemplate.Metallic))
                {
                    SetFloatRange(material, "_Metallic", standardMaterialTemplate.Metallic, 0f, 1f);
                }
                //设置Smoothness
                if (!string.IsNullOrEmpty(standardMaterialTemplate.Smoothness))
                {
                    SetFloatRange(material, "_Glossiness", standardMaterialTemplate.Smoothness, 0f, 1f);
                }
                //设置EmissionColor
                if (!string.IsNullOrEmpty(standardMaterialTemplate.EmissionColor))
                {
                    if (standardMaterialTemplate.EmissionColor.Length == 6)
                    {
                        standardMaterialTemplate.EmissionColor += "00";
                    }
                    SetColor(material, "_EmissionColor", standardMaterialTemplate.EmissionColor);
                }
                //设置GlobalIllumination
                if (!string.IsNullOrEmpty(standardMaterialTemplate.GlobalIllumination))
                {
                    MaterialGlobalIlluminationFlags globalIlluminationFlags = EnumTools.GetEnum<MaterialGlobalIlluminationFlags>(standardMaterialTemplate.GlobalIllumination);
                    if (globalIlluminationFlags != (MaterialGlobalIlluminationFlags)(-1))
                    {
                        material.globalIlluminationFlags = globalIlluminationFlags;
                    }
                }
                //设置Emission
                if (!string.IsNullOrEmpty(standardMaterialTemplate.Emission))
                {
                    SetKeyword(material, "_EMISSION", standardMaterialTemplate.Emission);
                }
                //设置MainMaps Tiling X
                if (!string.IsNullOrEmpty(standardMaterialTemplate.MainMapsTilingX))
                {
                    float? x = StringToFloat(standardMaterialTemplate.MainMapsTilingX);
                    if (x != null)
                    {
                        material.mainTextureScale = new Vector2(x.Value, material.mainTextureScale.y);
                    }
                }
                //设置MainMaps Tiling Y
                if (!string.IsNullOrEmpty(standardMaterialTemplate.MainMapsTilingY))
                {
                    float? y = StringToFloat(standardMaterialTemplate.MainMapsTilingY);
                    if (y != null)
                    {
                        material.mainTextureScale = new Vector2(material.mainTextureScale.x, y.Value);
                    }
                }
                //设置MainMaps Offset X
                if (!string.IsNullOrEmpty(standardMaterialTemplate.MainMapsOffsetX))
                {
                    float? x = StringToFloat(standardMaterialTemplate.MainMapsOffsetX);
                    if (x != null)
                    {
                        material.mainTextureOffset = new Vector2(x.Value, material.mainTextureOffset.y);
                    }
                }
                //设置MainMaps Offset Y
                if (!string.IsNullOrEmpty(standardMaterialTemplate.MainMapsOffsetY))
                {
                    float? y = StringToFloat(standardMaterialTemplate.MainMapsOffsetY);
                    if (y != null)
                    {
                        material.mainTextureOffset = new Vector2(material.mainTextureOffset.x, y.Value);
                    }
                }
                //设置SpecularHighlights
                if (!string.IsNullOrEmpty(standardMaterialTemplate.SpecularHighlights))
                {
                    bool? specularHighlights = StringToBool(standardMaterialTemplate.SpecularHighlights);
                    if (specularHighlights != null)
                    {
                        if (specularHighlights.Value)
                        {
                            material.SetFloat("_SpecularHighlights", 1f);
                        }
                        else
                        {
                            material.SetFloat("_SpecularHighlights", 0f);
                        }
                    }
                }
                //设置Reflections
                if (!string.IsNullOrEmpty(standardMaterialTemplate.Reflections))
                {
                    bool? reflections = StringToBool(standardMaterialTemplate.Reflections);
                    if (reflections != null)
                    {
                        if (reflections.Value)
                        {
                            material.SetFloat("_GlossyReflections", 1f);
                        }
                        else
                        {
                            material.SetFloat("_GlossyReflections", 0f);
                        }
                    }
                }
                //设置RenderQueue
                if (!string.IsNullOrEmpty(standardMaterialTemplate.RenderQueue))
                {
                    int renderQueue;
                    if (int.TryParse(standardMaterialTemplate.RenderQueue, out renderQueue))
                    {
                        material.renderQueue = renderQueue;
                    }
                }
                //设置EnableInstancing
                if (!string.IsNullOrEmpty(standardMaterialTemplate.EnableInstancing))
                {
                    bool? enableInstancing = StringToBool(standardMaterialTemplate.EnableInstancing);
                    if (enableInstancing != null)
                    {
                        material.enableInstancing = enableInstancing.Value;
                    }

                }

            }
        }
    }

    private static void SetMaterialRenderingMode(Material material, string renderingMode)
    {
        switch (renderingMode)
        {
            case "Opaque":
                material.SetFloat("_Mode", 0);
                material.SetInt("_SrcBlend", 1);
                material.SetInt("_DstBlend", 0);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case "Cutout":
                material.SetFloat("_Mode", 1);
                material.SetInt("_SrcBlend", 1);
                material.SetInt("_DstBlend", 0);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case "Fade":
                material.SetFloat("_Mode", 2);
                material.SetInt("_SrcBlend", 5);
                material.SetInt("_DstBlend", 10);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case "Transparent":
                material.SetFloat("_Mode", 3);
                material.SetInt("_SrcBlend", 1);
                material.SetInt("_DstBlend", 10);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }

    private static void SetColor(Material material, string name, string value)
    {
        Color? color = HexColorStringToColor(value);
        if (color != null)
        {
            material.SetColor(name, color.Value);
        }
    }

    private static void SetFloatRange(Material material, string name, string value, float min, float max)
    {
        float? retValue = StringToFloat(value);
        if (retValue!=null && min <= retValue && retValue <= max)
        {
            material.SetFloat(name, retValue.Value);
        }
    }

    private static void SetKeyword(Material material, string name, string value)
    {
        bool? retValue = StringToBool(value);
        if (retValue!=null)
        {
            if (retValue.Value)
            {
                material.EnableKeyword(name);
            }
            else
            {
                material.DisableKeyword(name);
            }
        }
    }

    private static bool? StringToBool(string boolStr)
    {
        bool ret;
        if (bool.TryParse(boolStr, out ret))
        {
            return ret;
        }
        return null;
    }


    private static float? StringToFloat(string floatStr)
    {
        float ret;
        if (float.TryParse(floatStr, out ret))
        {
            return ret;
        }
        return null;
    }


    private static Color? HexColorStringToColor(string colorString)
    {
        try
        {
            var result = Regex.Matches(colorString, @"\w{2}");
            List<string> re = new List<string>();
            foreach (Match item in result)
            {
                re.Add(item.Value);
            }
            byte r = Convert.ToByte(re[0], 16);
            byte g = Convert.ToByte(re[1], 16);
            byte b = Convert.ToByte(re[2], 16);
            byte a = Convert.ToByte(re[3], 16);
            return new Color32(r, g, b, a);
        }
        catch (Exception)
        {
            return null;
        }

    }
}

[Serializable]
public class StandardMaterialTemplate
{
    [SerializeField] public string RenderingMode;       //枚举 Opaque|Cutout|Fade|Transparent
    [SerializeField] public string MainMapsColor;       //16进制颜色 例如:FF7036FF
    [SerializeField] public string AlphaCutoff;         //float:[0,1]
    [SerializeField] public string Smoothness;          //float:[0,1]
    [SerializeField] public string Metallic;            //float:[0,1]
    [SerializeField] public string Emission;            //bool:true|false
    [SerializeField] public string EmissionColor;       //float:[0,1]
    [SerializeField] public string GlobalIllumination;  //枚举 None|RealtimeEmissive|BakedEmissive|AnyEmissive|EmissiveIsBlack
    [SerializeField] public string MainMapsTilingX;     //float
    [SerializeField] public string MainMapsTilingY;     //float
    [SerializeField] public string MainMapsOffsetX;     //float
    [SerializeField] public string MainMapsOffsetY;     //float
    [SerializeField] public string SpecularHighlights;  //bool
    [SerializeField] public string Reflections;         //bool
    [SerializeField] public string RenderQueue;         //int
    [SerializeField] public string EnableInstancing;    //int
}
