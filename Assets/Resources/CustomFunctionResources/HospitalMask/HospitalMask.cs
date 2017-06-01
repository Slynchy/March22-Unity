using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomFunctions
{
    public class HospitalMask : CustomFunction
    {
        GameObject HospitalMaskPrefab;
        GameObject HospitalMaskInstance;
        HospitalMaskObjectScript HospitalMaskScript;
        M22.ScriptMaster ScriptMaster;

        public override void Awake()
        {
            HospitalMaskPrefab = Resources.Load<GameObject>("CustomFunctionResources/HospitalMask/HospitalMaskPrefab") as GameObject;
        }

        public override void Start()
        {
            ScriptMaster = Camera.main.GetComponent<M22.ScriptMaster>();
        }

        public override void Func(string param1, string param2, string param3)
        {
            if(param1.Equals("show"))
            {
                HospitalMaskInstance = GameObject.Instantiate<GameObject>(HospitalMaskPrefab, ScriptMaster.GetCanvas(M22.ScriptMaster.CANVAS_TYPES.PRECHARACTER).transform);
                HospitalMaskScript = HospitalMaskInstance.GetComponent<HospitalMaskObjectScript>();
            }
            else
            {
                HospitalMaskInstance = GameObject.Find("HospitalMaskInstance");
                if (HospitalMaskInstance != null) GameObject.Destroy(HospitalMaskInstance);
            }
        }
    }
}