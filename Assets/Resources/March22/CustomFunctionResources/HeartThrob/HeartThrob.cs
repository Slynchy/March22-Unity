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

            public override void Func(string[] _params)
            {
                HeartThrobInstance = GameObject.Instantiate<GameObject>(HeartThrobPrefab, GameObject.Find("EffectCanvas").transform);
                HeartThrobScript = HeartThrobInstance.GetComponent<HeartThrobObjectScript>();
                HeartThrobScript.fadeInSpeed = float.Parse(_params[0]);
                HeartThrobScript.fadeOutSpeed = float.Parse(_params[1]);
                HeartThrobScript.callback = Camera.main.GetComponent<M22.ScriptMaster>().NextLine;
            }
        }
    }
}