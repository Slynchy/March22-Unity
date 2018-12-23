using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace M22
{
    namespace Functions
    {
        public class DrawCharacter : InternalFunction
        {

            public override void Awake()
            {

            }

            public override void Func(ref line_c line, bool _isInLine)
            {
                this.scriptMaster.getVNHandler().CreateCharacter(line.m_parameters_txt[0], line.m_parameters_txt[1], line.m_parameters[0]);
                if (line.m_parameters[1] != 1)
                {
                    this.scriptMaster.HideText();
                    //WaitState = WAIT_STATE.CHARACTER_FADEIN;
                    this.scriptMaster.WaitQueue.Add(new WaitObject(WAIT_STATE.CHARACTER_FADEIN));
                }
                else
                    this.scriptMaster.NextLine(_isInLine);
            }
        }
    }
}