using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M22
{

    public class CustomFunctionHandler : MonoBehaviour
    {

        public delegate void CustomFunctionDelegate(string[] _params);

        struct CustomFunctionContainer
        {
            public string name;
            public CustomFunction CustomFunc;
            public CustomFunctionContainer(string _name, CustomFunction _CustomFunc)
            {
                name = _name;
                CustomFunc = _CustomFunc;
            }
        }

        static public bool CheckFunctionExists(string _str)
        {
            return CustomFuncs.ContainsKey(_str);
        }

        static private Dictionary<string, CustomFunctionContainer> CustomFuncs = new Dictionary<string, CustomFunctionContainer>();

        static public CustomFunctionDelegate GetFunction(string name)
        {
            CustomFunctionContainer returnVal;
            if (CustomFuncs.TryGetValue(name, out returnVal) == false)
            {
                Debug.LogErrorFormat("Failed to execute custom function {0}", name);
                return null;
            }
            return returnVal.CustomFunc.Func;
        }

        private void Awake()
        {
            // ADD YOUR CUSTOM FUNCTIONS DOWN HERE
            CustomFuncs.Add("heart_throb", new CustomFunctionContainer("heart_throb", new M22.CustomFunctions.HeartThrob()));
            CustomFuncs.Add("snow_effect", new CustomFunctionContainer("snow_effect", new M22.CustomFunctions.SnowEffect()));
            CustomFuncs.Add("HospitalMask", new CustomFunctionContainer("HospitalMask", new M22.CustomFunctions.HospitalMask()));
            CustomFuncs.Add("WrittenNote", new CustomFunctionContainer("WrittenNote", new M22.CustomFunctions.WrittenNote()));
            CustomFuncs.Add("sakura_effect", new CustomFunctionContainer("sakura_effect", new M22.CustomFunctions.SakuraEffect()));
            CustomFuncs.Add("DrawSprite", new CustomFunctionContainer("DrawSprite", new M22.CustomFunctions.DrawSprite()));

            foreach (var item in CustomFuncs)
            {
                item.Value.CustomFunc.Awake();
            }
        }

        private void Start()
        {
            foreach (var item in CustomFuncs)
            {
                item.Value.CustomFunc.Start();
            }
        }
    }

    abstract public class CustomFunction
    {
        virtual public void Awake() { }
        virtual public void Start() { }
        virtual public void Func(string[] _params) { }
    }
}