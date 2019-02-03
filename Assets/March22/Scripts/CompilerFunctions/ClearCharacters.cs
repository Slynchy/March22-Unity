using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace M22
{
    namespace Functions
    {
        public class ClearCharacters : InternalFunction
        {

            public override void Awake()
            {

            }

            public override void Func(ref line_c line, bool _isInLine)
            {
                this.scriptMaster.getVNHandler().ClearCharacters(line.m_parameters[0] == 1 ? true : false);
                this.scriptMaster.HideText();
                this.scriptMaster.WaitQueue.Add(new WaitObject(WAIT_STATE.CHARACTER_FADEOUT));
            }
        }
    }
}