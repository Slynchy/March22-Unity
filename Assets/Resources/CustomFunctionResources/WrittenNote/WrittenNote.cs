using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M22
{
    namespace CustomFunctions
    {
        public class WrittenNote : CustomFunction
        {
            GameObject WrittenNoteInstance;
            GameObject WrittenNotePrefab;
            M22.ScriptMaster ScriptMaster;

            public override void Awake()
            {
                WrittenNotePrefab = Resources.Load<GameObject>("CustomFunctionResources/WrittenNote/WrittenNotePrefab") as GameObject;
            }

            public override void Start()
            {
                ScriptMaster = Camera.main.GetComponent<M22.ScriptMaster>();
            }

            public override void Func(string param1, string param2, string param3)
            {
                string param_fixed = param1.Replace('_', ' ');
                WrittenNoteInstance = GameObject.Instantiate<GameObject>(WrittenNotePrefab, ScriptMaster.GetCanvas(M22.ScriptMaster.CANVAS_TYPES.EFFECTS).transform);

                ScriptMaster.NextLine();
            }
        }
    }
}