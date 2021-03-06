﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M22
{
    namespace CustomFunctions
    {
        public class HospitalMask : CustomFunction
        {
            GameObject HospitalMaskPrefab;
            GameObject HospitalMaskInstance;

            public override string Keyword()
            {
                return "HospitalMask";
            }

            public override void Awake()
            {
                HospitalMaskPrefab = Resources.Load<GameObject>("CustomFunctionResources/HospitalMask/HospitalMaskPrefab") as GameObject;
            }

            public override void Func(string[] _params)
            {
                if (_params[0].Equals("show"))
                {
                    HospitalMaskInstance = GameObject.Instantiate<GameObject>(HospitalMaskPrefab, this.scriptMaster.GetCanvas(M22.ScriptMaster.CANVAS_TYPES.POSTCHARACTER).GetComponent<RectTransform>().transform) as GameObject;
                }
                else
                {
                    if (HospitalMaskInstance == null)
                        Debug.Log("Can't delete mask, there is no mask!");
                    else
                        GameObject.Destroy(HospitalMaskInstance);
                }
                this.scriptMaster.NextLine();
            }
        }
    }
}