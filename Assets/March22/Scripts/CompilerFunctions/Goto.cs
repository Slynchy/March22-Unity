using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace M22
{
    namespace Functions
    {
        public class Goto : InternalFunction
        {

            public override void Awake()
            {

            }

            public override void Func(ref line_c line, bool isInline)
            {
                bool success = false;
                foreach (var item in M22.ScriptCompiler.currentScript_checkpoints)
                {
                    if (item.m_name == line.m_parameters_txt[0])
                    {
                        success = true;
                        this.scriptMaster.GotoLine(item.m_position);
                        break;
                    }
                }
                if (!success)
                {
                    Debug.LogError("Failed to find checkpoint: " + line.m_parameters_txt[0][0]);
                }
            }
        }
    }
}