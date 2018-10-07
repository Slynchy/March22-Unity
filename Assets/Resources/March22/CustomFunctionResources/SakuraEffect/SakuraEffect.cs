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
                ScriptMaster = Camera.main.GetComponent<M22.SceneManager>().ScriptMaster;
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
                        var gameObjects = GameObject.FindObjectsOfType<GameObject>() as GameObject[];

                        for (var i = 0; i < gameObjects.Length; i++)
                        {
                            if (gameObjects[i].name.Contains("SakuraPrefab"))
                            {
                                SakuraEffectInstance = gameObjects[i];
                                SakuraEffectScript = SakuraEffectInstance.GetComponent<SakuraEffectObjectScript>();
                                break;
                            }
                        }

                        if (!SakuraEffectInstance || !SakuraEffectScript)
                        {
                            Debug.LogError("Trying to stop SakuraEffect when there isn't one!");
                            ScriptMaster.NextLine();
                            return;
                        }
                    }
                    SakuraEffectScript.Stop();
                }

                ScriptMaster.NextLine();

            }
        }
    }
}