using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFunctionHandler : MonoBehaviour {

    public delegate void CustomFunctionDelegate(string param1, string param2, string param3);

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

    static private Dictionary<string, CustomFunctionContainer> CustomFuncs = new Dictionary<string, CustomFunctionContainer>();

    static public CustomFunctionDelegate GetFunction(string name)
    {
        CustomFunctionContainer returnVal;
        CustomFuncs.TryGetValue(name, out returnVal);
        return returnVal.CustomFunc.Func;
    }

    private void Awake()
    {
        // ADD YOUR CUSTOM FUNCTIONS DOWN HERE
        CustomFuncs.Add("heart_throb", new CustomFunctionContainer("heart_throb", new CustomFunctions.HeartThrob()));
        CustomFuncs.Add("snow_effect", new CustomFunctionContainer("snow_effect", new CustomFunctions.SnowEffect()));
        CustomFuncs.Add("HospitalMask", new CustomFunctionContainer("HospitalMask", new CustomFunctions.HospitalMask()));

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
    virtual public void Func(string param1, string param2, string param3) { }
}