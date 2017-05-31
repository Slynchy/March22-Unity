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

        public override void Awake()
        {
            HospitalMaskPrefab = Resources.Load<GameObject>("CustomFunctionResources/HospitalMask/HospitalMaskPrefab") as GameObject;
        }

        public override void Func(string param1, string param2, string param3)
        {
            if(param1.Equals("show"))
            {
                HospitalMaskInstance = GameObject.Instantiate<GameObject>(HospitalMaskPrefab, GameObject.Find("Background").transform);
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