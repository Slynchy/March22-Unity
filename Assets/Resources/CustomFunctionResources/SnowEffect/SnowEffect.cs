using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M22
{
    namespace CustomFunctions
    {
        public class SnowEffect : CustomFunction
        {
            GameObject SnowEffectPrefab;
            GameObject SnowEffectInstance;
            SnowEffectObjectScript SnowEffectScript;

            public override void Awake()
            {
                SnowEffectPrefab = Resources.Load<GameObject>("CustomFunctionResources/SnowEffect/SnowEffectPrefab") as GameObject;
            }

            public override void Func(string param1, string param2, string param3)
            {
                bool stopOrStart = false; // false == stop, true == start
                if (param1.Equals("start"))
                    stopOrStart = true;
                else if (param1.Equals("stop"))
                    stopOrStart = false;
                else
                {
                    Debug.LogError("Invalid parameter on SnowEffect function!");
                    Camera.main.GetComponent<M22.ScriptMaster>().NextLine();
                    return;
                }

                // starting
                if (stopOrStart == true)
                {
                    SnowEffectInstance = GameObject.Instantiate<GameObject>(SnowEffectPrefab, GameObject.Find("PostCharacterEffectCanvas").transform);
                    SnowEffectScript = SnowEffectInstance.GetComponent<SnowEffectObjectScript>();
                }
                else // stopping
                {
                    if (SnowEffectInstance == null || SnowEffectScript == null)
                    {
                        Debug.LogError("Trying to stop SnowEffect when there isn't one!");
                        Camera.main.GetComponent<M22.ScriptMaster>().NextLine();
                        return;
                    }
                    SnowEffectScript.Stop();
                }

                Camera.main.GetComponent<M22.ScriptMaster>().NextLine();

            }
        }
    }
}