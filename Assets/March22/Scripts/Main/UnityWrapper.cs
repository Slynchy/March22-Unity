using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityWrapper
{

    public static string LoadTextFileAsString(string _path)
    {
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, _path + ".txt");
        string temp = "";
        try
        {
            temp = System.IO.File.ReadAllText(filePath);
        }
        catch(System.Exception exc)
        {
            temp = "";
        }
        if(temp == "" || temp == null)
        {
            var temp2 = (Resources.Load(_path) as TextAsset);
            if (temp2 != null)
                return temp2.text;
            else
                return "";
        }
        else
        {
            return temp;
        }
    }

    public static void LogError(string _error)
    {
        Debug.LogError(_error);
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
        Debug.LogErrorFormat(format, args);
    }
    public static void LogWarningFormat(string format, params object[] args)
    {
        Debug.LogWarningFormat(format, args);
    }

    public struct Color
    {
        public float a;
        public float b;
        public float g;
        public float r;
        public Color(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = 1;
        }
        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    public struct Color32
    {
        public byte a;
        public byte b;
        public byte g;
        public byte r;
        public Color32(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = 1;
        }
        public Color32(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        static public implicit operator UnityEngine.Color(Color32 input)
        {
            UnityEngine.Color32 temp = new UnityEngine.Color32(input.r, input.g, input.b, input.a);
            return temp;
        }
        static public implicit operator UnityEngine.Color32(Color32 input)
        {
            return new UnityEngine.Color32(input.r, input.g, input.b, input.a);
        }
    }
}