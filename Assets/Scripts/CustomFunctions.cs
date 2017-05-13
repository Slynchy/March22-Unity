using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFunctions : MonoBehaviour {

    public delegate void CustomFunctionDelegate(string param1, string param2, string param3);

    //struct CustomFunction
    //{
    //    public CustomFunctionDelegate Func;
    //    public string[] parameters_txt;
    //}

    static private Dictionary<string, CustomFunctionDelegate> CustomFuncs = new Dictionary<string, CustomFunctionDelegate>();

    static public CustomFunctionDelegate GetFunction(string name)
    {
        CustomFunctionDelegate returnVal;
        CustomFuncs.TryGetValue(name, out returnVal);
        return returnVal;
    }

    private void Awake()
    {
        // ADD YOUR CUSTOM FUNCTIONS DOWN HERE
        CustomFuncs.Add("heart_throb", HeartThrob);
    }

    private void HeartThrob(string param1, string param2, string param3)
    {
        Debug.Log("Custom functionality!");
        Camera.main.GetComponent<M22.ScriptMaster>().NextLine();
        return;
    }
}
