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

            public override string Keyword()
            {
                return "snow_effect";
            }

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
                    this.scriptMaster.NextLine();
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
                        var gameObjects = GameObject.FindObjectsOfType<GameObject>() as GameObject[];

                        for (var i = 0; i < gameObjects.Length; i++)
                        {
                            if (gameObjects[i].name.Contains("SakuraPrefab"))
                            {
                                SnowEffectInstance = gameObjects[i];
                                SnowEffectScript = SnowEffectInstance.GetComponent<SnowEffectObjectScript>();
                                break;
                            }
                        }

                        if (!SnowEffectInstance || !SnowEffectScript)
                        {
                            Debug.LogError("Trying to stop SnowEffect when there isn't one!");
                            this.scriptMaster.NextLine();
                            return;
                        }
                    }
                    SnowEffectScript.Stop();
                }

                this.scriptMaster.NextLine();

            }
        }
    }
}