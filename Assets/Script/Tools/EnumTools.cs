using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumTools
{
    public static T GetEnum<T>(string e) where T : struct, IConvertible
    {
        try
        {
            return (T)Enum.Parse(typeof(T), e);

        }
        catch (Exception)
        {
            return GetException<T>();
        }

    }

    public static T GetException<T>() where T : struct, IConvertible
    {
        object obj = (-10086);
        return (T)(obj);
    }

    public static string GetString<T>(T v) where T : struct, IConvertible
    {
        return Enum.GetName(typeof(T), v);
    }
}
