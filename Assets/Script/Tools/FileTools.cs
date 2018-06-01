using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class FileTools
{
    /// <summary>
    /// 判断指定路径的目录是否存在
    /// </summary>
    /// <param name="path">要测试的路径。</param>
    /// <returns>如果 path 指向现有目录，则为 true；如果该目录不存在或者在尝试确定指定目录是否存在时出错，则为 false。</returns>
    public static bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    /// <summary>
    /// 判断指定的文件是否存在。
    /// </summary>
    /// <param name="path">要检查的文件。</param>
    /// <returns>如果调用方具有要求的权限并且 true 包含现有文件的名称，则为 path；否则为 false。 如果 false 为 path（一个无效路径或零长度字符串）,则此方法也将返回 null。 如果调用方不具有读取指定文件所需的足够权限，则不引发异常并且该方法返回 false，这与 path 是否存在无关。</returns>
    public static bool FileExists(string path)
    {
        return File.Exists(path);
    }

    /// <summary>
    /// 返回指定路径字符串的文件名和扩展名。
    /// </summary>
    /// <param name="path">从中获取文件名和扩展名的路径字符串。</param>
    /// <returns>path 中最后一个目录字符后的字符。 如果 path 的最后一个字符是目录或卷分隔符，则此方法返回 String.Empty。 如果 path 为 null，则此方法返回 null。</returns>
    public static string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }

    /// <summary>
    /// 返回不具有扩展名的指定路径字符串的文件名。
    /// </summary>
    /// <param name="path">文件的路径。</param>
    /// <returns>返回的字符串 GetFileName, ，减去最后的句点 （.） 和之后的所有字符。</returns>
    public static string GetFileNameWithoutExtension(string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }

    public static string GetDirectoryName(string path)
    {
        return Path.GetDirectoryName(path);
    }

    /// <summary>
    /// 得到指定路径的扩展名,指定路径的扩展名（不包含句点“.”）、或 null、或 String.Empty。 如果 path 为 null，则 GetExtension 返回 null。 如果 path 不具有扩展名信息，则 GetExtension 返回 String.Empty。
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetExtension(string path)
    {
        string ret = Path.GetExtension(path);
        if (!string.IsNullOrEmpty(ret))
        {
            ret = ret.Substring(1);
        }
        return ret;
    }

    /// <summary>
    /// 在指定路径中创建所有目录和子目录，除非它们已经存在。
    /// </summary>
    /// <param name="path">要创建的目录。</param>
    public static void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    /// <summary>
    /// 将两个字符串组合成一个路径。
    /// </summary>
    /// <param name="path1">要组合的第一个路径。</param>
    /// <param name="path2">要组合的第二个路径。</param>
    /// <returns>已组合的路径。 如果指定的路径之一是零长度字符串，则该方法返回其他路径。 如果 path2 包含绝对路径，则该方法返回 path2。</returns>
    public static string PathCombine(string path1, string path2)
    {
        return Path.Combine(path1, path2);
    }

    /// <summary>
    /// 删除指定的文件。
    /// </summary>
    /// <param name="path">要删除的文件的名称。 该指令不支持通配符。</param>
    public static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// 删除指定目录以及所有子目录和子文件
    /// </summary>
    /// <param name="path"></param>
    public static void DeleteDirectory(string path)
    {
        if (DirectoryExists(path))
        {
            Directory.Delete(path, true);
        }
    }

    /// <summary>
    /// 将指定路径的文件重命名
    /// </summary>
    /// <param name="path">文件的路径</param>
    /// <param name="newName">文件新名称</param>
    public static void RenameFile(string path, string newName)
    {
        File.Move(path, Path.Combine(GetDirectoryName(path), newName));
    }

    /// <summary>
    /// 使用UTF8编码方式从文件读取内容
    /// </summary>
    /// <param name="path">要读取的文件的路径</param>
    /// <returns>返回读取的内容</returns>
    public static string ReadFileUTf8(string path)
    {
        try
        {
            return Encoding.UTF8.GetString(ReadFileToBytes(path));
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }

    /// <summary>
    /// 使用UTF8编码方式从文件读取内容
    /// </summary>
    /// <param name="path">要读取的文件的路径</param>
    /// <returns>返回读取的内容</returns>
    public static byte[] ReadFileToBytes(string path)
    {
        FileStream stream = null;
        try
        {
            stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] ret = new byte[FileSize(path)];
            stream.Read(ret, 0, ret.Length);
            return ret;
        }
        finally
        {
            if (stream != null)
            {
                stream.Dispose();
            }
        }
    }

    /// <summary>
    /// 用UTF8的方式往文件里面写入内容,如果制定路径已经存在该文件,则会覆盖该文件
    /// </summary>
    /// <param name="path">要写入的文件的路径</param>
    /// <param name="content">要写入的内容</param>
    public static void WriteFileUtf8Create(string path, string content)
    {
        if (!DirectoryExists(GetDirectoryName(path)))
        {
            CreateDirectory(GetDirectoryName(path));
        }
        FileStream stream = null;
        try
        {
            stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            stream.Write(buffer, 0, buffer.Length);
        }
        finally
        {
            if (stream != null)
            {
                stream.Flush();
                stream.Dispose();
            }
        }
    }

    /// <summary>
    /// 用UTF8的方式往文件里面写入内容,如果制定路径已经存在该文件,则会追加内容
    /// </summary>
    /// <param name="path">要写入的文件的路径</param>
    /// <param name="content">要写入的内容</param>
    public static void WriteFileUtf8Append(string path, string content)
    {
        if (!DirectoryExists(GetDirectoryName(path)))
        {
            CreateDirectory(GetDirectoryName(path));
        }
        FileStream stream = null;
        try
        {
            stream = new FileStream(path, FileMode.Append, FileAccess.Write);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            stream.Write(buffer, 0, buffer.Length);
        }
        finally
        {
            if (stream != null)
            {
                stream.Flush();
                stream.Dispose();
            }
        }
    }

    /// <summary>
    /// 以字节的形式往文件中写入数据,文件已经存在时,将在文件尾部追加数据
    /// </summary>
    /// <param name="path">文件的路径</param>
    /// <param name="content">要写入的数据</param>
    public static void WriteFileBytesAppend(string path, byte[] content)
    {
        if (!DirectoryExists(GetDirectoryName(path)))
        {
            CreateDirectory(GetDirectoryName(path));
        }
        FileStream stream = null;
        try
        {
            stream = new FileStream(path, FileMode.Append, FileAccess.Write);
            stream.Write(content, 0, content.Length);
        }
        finally
        {
            if (stream != null)
            {
                stream.Flush();
                stream.Dispose();
            }
        }
    }

    /// <summary>
    /// 返回指定路径的文件的大小,文件不存在时返回0
    /// </summary>
    /// <param name="path">文件的路径</param>
    /// <returns>指定文件的大小,不存在时返回0</returns>
    public static long FileSize(string path)
    {
        if (FileExists(path))
        {
            return new System.IO.FileInfo(path).Length;
        }
        return 0;
    }

    /// <summary>
    /// 返回指定目录中所有子文件的路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[] GetFiles(string path)
    {
        return Directory.GetFiles(path);
    }

    /// <summary>
    /// 返回指定路径中的所有文件和子目录的名称。
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[] GetFileSystemEntries(string path)
    {
        if (!DirectoryExists(path))
        {
            return new string[0];
        }
        return Directory.GetFileSystemEntries(path);
    }

    public static void MoveFile(string sourceFileName, string destFileName)
    {
        if (FileExists(destFileName))
        {
            DeleteFile(destFileName);
        }
        File.Move(sourceFileName, destFileName);
    }

    public static void CopyFile(string sourceFileName, string destFileName)
    {
        if (FileExists(destFileName))
        {
            DeleteFile(destFileName);
        }
        File.Copy(sourceFileName, destFileName);
    }

    public static string GetStreamingAssetsPath()
    {

#if UNITY_EDITOR
        return Application.streamingAssetsPath;
#elif UNITY_IPHONE
        return Application.dataPath + "/Raw";
#elif UNITY_ANDROID
        return Application.dataPath + "/!/assets";
#elif UNITY_STANDALONE_WIN
        return Application.streamingAssetsPath;
#else
        return Application.streamingAssetsPath;
#endif
    }

}
