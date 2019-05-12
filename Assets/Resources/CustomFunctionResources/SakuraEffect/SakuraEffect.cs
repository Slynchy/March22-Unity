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

            public override string Keyword()
            {
                return "sakura_effect";
            }

            public override void Awake()
            {
                SakuraEffectPrefab = Resources.Load<GameObject>("CustomFunctionResources/SakuraEffect/SakuraPrefab") as GameObject;
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
                    SakuraEffectInstance = GameObject.Instantiate<GameObject>(SakuraEffectPrefab, this.scriptMaster.GetCanvas(M22.ScriptMaster.CANVAS_TYPES.POSTCHARACTER).transform);
                }
                else // stopping
                {
                    if (SakuraEffectInstance == null)
                    {
                        var gameObjects = GameObject.FindObjectsOfType<GameObject>() as GameObject[];

                        for (var i = 0; i < gameObjects.Length; i++)
                        {
                            if (gameObjects[i].name.Contains("SakuraPrefab"))
                            {
                                SakuraEffectInstance = gameObjects[i];
                                break;
                            }
                        }

                        if (SakuraEffectInstance == null)
                        {
                            Debug.LogError("Trying to stop SakuraEffect when there isn't one!");
                            this.scriptMaster.NextLine();
                            return;
                        }
                    }
                    GameObject.Destroy(SakuraEffectInstance);
                }

                this.scriptMaster.NextLine();

            }
        }
    }
}