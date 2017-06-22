using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M22
{
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
                HeartThrobInstance = GameObject.Instantiate<GameObject>(HeartThrobPrefab, GameObject.Find("EffectCanvas").transform);
                HeartThrobScript = HeartThrobInstance.GetComponent<HeartThrobObjectScript>();
                HeartThrobScript.fadeInSpeed = float.Parse(param1);
                HeartThrobScript.fadeOutSpeed = float.Parse(param2);
                HeartThrobScript.callback = Camera.main.GetComponent<M22.ScriptMaster>().NextLine;
            }
        }
    }
}