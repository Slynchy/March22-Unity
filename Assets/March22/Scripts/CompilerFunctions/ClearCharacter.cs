using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace M22
{
    namespace Functions
    {
        public class ClearCharacter : InternalFunction
        {

            public override void Awake()
            {

            }

            public override void Func(ref line_c line, bool _isInLine)
            {
                if (!this.scriptMaster.getVNHandler().ClearCharacter(line.m_parameters_txt[0], (line.m_parameters[0] == 1)))
                {
                    Debug.LogErrorFormat("Unable to clear character {0} at line {1}", line.m_parameters_txt[0], line.m_origScriptPos);
                    this.scriptMaster.NextLine(_isInLine);
                    return;
                }
                if (line.m_parameters[0] == 1)
                    this.scriptMaster.NextLine(_isInLine);
                else
                    this.scriptMaster.WaitQueue.Add(new WaitObject(WAIT_STATE.CHARACTER_FADEOUT_INDIVIDUAL, line.m_parameters_txt[0]));
            }
        }
    }
}