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

            public override void Func(string[] _params)
            {
                bool stopOrStart = false; // false == stop, true == start
                if (_params[0].Equals("start"))
                    stopOrStart = true;
                else if (_params[0].Equals("stop"))
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