using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M22
{
    namespace CustomFunctions
    {
        public class SakuraEffect : CustomFunction
        {
            GameObject SakuraEffectPrefab;
            GameObject SakuraEffectInstance;
            SakuraEffectObjectScript SakuraEffectScript;
            M22.ScriptMaster ScriptMaster;

            public override void Awake()
            {
                SakuraEffectPrefab = Resources.Load<GameObject>("CustomFunctionResources/SakuraEffect/SakuraPrefab") as GameObject;
            }

            public override void Start()
            {
                ScriptMaster = Camera.main.GetComponent<M22.ScriptMaster>();
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
                    ScriptMaster.NextLine();
                    return;
                }

                // starting
                if (stopOrStart == true)
                {
                    SakuraEffectInstance = GameObject.Instantiate<GameObject>(SakuraEffectPrefab, ScriptMaster.GetCanvas(M22.ScriptMaster.CANVAS_TYPES.POSTCHARACTER).transform);
                    SakuraEffectScript = SakuraEffectInstance.GetComponent<SakuraEffectObjectScript>();
                }
                else // stopping
                {
                    if (SakuraEffectInstance == null || SakuraEffectScript == null)
                    {
                        Debug.LogError("Trying to stop SnowEffect when there isn't one!");
                        ScriptMaster.NextLine();
                        return;
                    }
                    SakuraEffectScript.Stop();
                }

                ScriptMaster.NextLine();

            }
        }
    }
}