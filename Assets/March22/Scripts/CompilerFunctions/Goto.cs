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
                var index = this.scriptMaster.getCurrentScript().GetCheckpointIndex(line.m_parameters_txt[0]);
                if(index == -1)
                {
                    Debug.LogError("Failed to find checkpoint: " + line.m_parameters_txt[0]);
                } else
                {
                    this.scriptMaster.GotoLine(index);
                }
            }
        }
    }
}