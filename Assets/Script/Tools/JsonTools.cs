using UnityEngine;



public class JsonTools
{
    /// <summary>
    /// 从文件里面将Json文件解析为指定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T ResolutionJsonFromFile<T>(string path)
    {
        string json = "{}";
        if (FileTools.FileExists(path))
        {
            json = FileTools.ReadFileUTf8(path);
        }
        if (string.IsNullOrEmpty(json))
        {
            json = "{}";
        }
        T t = default(T);
        try
        {
            t = JsonUtility.FromJson<T>(json);
        }
        catch (System.ArgumentException)
        {
            Debug.Log(path + "路径下的Json文件有误");
            t = JsonUtility.FromJson<T>("{}");
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            t = JsonUtility.FromJson<T>("{}");
        }
        return t;
    }

    /// <summary>
    /// 从Json字符串里面将Json文件解析为指定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T ResolutionJsonFromString<T>(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            json = "{}";
        }
        T t = default(T);
        try
        {
            t = JsonUtility.FromJson<T>(json);
        }
        catch (System.ArgumentException)
        {
            Debug.Log("Json文件有误");
            t = JsonUtility.FromJson<T>("{}");
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            t = JsonUtility.FromJson<T>("{}");
        }
        return t;
    }

    public static void WriteJsonToFile(object o, string path)
    {
        string json = JsonUtility.ToJson(o);
        FileTools.WriteFileUtf8Create(path, json);
    }
}
