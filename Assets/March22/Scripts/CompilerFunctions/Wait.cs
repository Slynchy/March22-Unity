using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace M22
{
    namespace Functions
    {
        public class Wait : InternalFunction
        {

            public override void Awake()
            {

            }

            public override void Func(ref line_c line, bool _isInLine)
            {
                this.scriptMaster.WaitQueue.Add(new WaitObject(WAIT_STATE.WAIT_COMMAND));
                this.scriptMaster.setWaitCommandTimer(0);
            }
        }
    }
}