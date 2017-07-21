using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M22
{
    namespace CustomFunctions
    {
        public class WrittenNote : CustomFunction
        {
            GameObject WrittenNoteInstance;
            GameObject WrittenNotePrefab;
            ScriptMaster ScriptMaster;

            public override void Awake()
            {
                WrittenNotePrefab = Resources.Load<GameObject>("CustomFunctionResources/WrittenNote/NotePrefab") as GameObject;
            }

            public override void Start()
            {
                ScriptMaster = Camera.main.GetComponent<M22.ScriptMaster>();
            }

            // param1 = in or out
            // param2 = text, delimited by underscores
            public override void Func(string[] _params)
            {
                if(_params[0].Equals("out"))
                {
                    if (WrittenNoteInstance == null)
                    {
                        UnityWrapper.LogWarningFormat("WrittenNoteInstance is null; trying to fade out when there isn't a note?");
                    }
                    else
                    {
                        WrittenNoteInstance.GetComponent<WrittenNoteObjectScript>().HideNote();
                        WrittenNoteInstance = null;
                    }
                }
                else
                {
                    string param2_fixed = _params[1].Replace('_', ' ');
                    param2_fixed = param2_fixed.Replace("\\n", "\n");
                    WrittenNoteInstance = GameObject.Instantiate<GameObject>(WrittenNotePrefab, ScriptMaster.GetCanvas(M22.ScriptMaster.CANVAS_TYPES.POSTCHARACTER).transform);
                    WrittenNoteInstance.GetComponentInChildren<Text>().text = param2_fixed;
                }
            }
        }
    }
}