using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFunctions : MonoBehaviour {

    private delegate void CustomFunctionDelegate();

    private Dictionary<string, CustomFunctionDelegate> CustomFuncs;

    private void Awake()
    {
        CustomFuncs = new Dictionary<string, CustomFunctionDelegate>();

        // ADD YOUR CUSTOM FUNCTIONS DOWN HERE
        CustomFuncs.Add("heart_throb", HeartThrob);
    }

    private void HeartThrob()
    {
        return;
    }
}
