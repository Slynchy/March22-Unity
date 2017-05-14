using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomFunctions
{
    public class HeartThrob : CustomFunction
    {
        GameObject HeartThrobPrefab;
        GameObject HeartThrobInstance;
        HeartThrobObjectScript HeartThrobScript;

        public override void Awake()
        {
            HeartThrobPrefab = Resources.Load<GameObject>("CustomFunctionResources/HeartThrob/HeartThrobPrefab") as GameObject;
        }

        public override void Func(string param1, string param2, string param3)
        {
            HeartThrobInstance = GameObject.Instantiate<GameObject>(HeartThrobPrefab, GameObject.Find("Canvas").transform);
            HeartThrobScript = HeartThrobInstance.GetComponent<HeartThrobObjectScript>();
            HeartThrobScript.callback = Camera.main.GetComponent<M22.ScriptMaster>().NextLine;
        }
    }
}