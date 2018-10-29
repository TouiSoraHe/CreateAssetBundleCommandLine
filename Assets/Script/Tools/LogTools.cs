using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LogTools {
    private static readonly string logFilePah = GlobalConstants.LogFilePath;
    private enum LogLevel { Info, Warning , Error }
    private static LogLevel loglevel = LogLevel.Info;



    public static void Info(string content)
    {
        loglevel = LogLevel.Info;
        string formatContent = FormatLog(content);
        FileTools.WriteFileUtf8Append(logFilePah, formatContent);
    }

    public static void Warning(string content)
    {
        loglevel = LogLevel.Warning;
        string formatContent = FormatLog(content);
        FileTools.WriteFileUtf8Append(logFilePah, formatContent);
    }

    public static void Error(string content, Exception e = null)
    {
        loglevel = LogLevel.Error;
        string formatContent = FormatLog(content, e);
        FileTools.WriteFileUtf8Append(logFilePah, formatContent);
    }

    /// <summary>
    /// 打印日志到Unity自身生成的日志文件中去,添加分割符号,方便使用正则表达式从日志文件中匹配出输出的信息
    /// </summary>
    /// <param name="error"></param>
    public static void PrintError(string error)
    {
        UnityEngine.Debug.Log(GlobalConstants.Delimiter + error + GlobalConstants.Delimiter);
    }

    private static string FormatLog(string content, Exception e = null)
    {
        string finallyContent = "";
        finallyContent += DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss")+" ";
        finallyContent += "[" + loglevel.ToString() + "] :";
        finallyContent += content;
        if (e !=null)
        {
            finallyContent += "\n\n";
            finallyContent += e.GetType().Name + ":" + e.Message+"\n";
            finallyContent += e.StackTrace;
        }
        finallyContent += "\n\n\n";
        return finallyContent;
    }
}
