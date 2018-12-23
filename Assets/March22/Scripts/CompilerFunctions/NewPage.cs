using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace M22
{
    namespace Functions
    {
        public class NewPage : InternalFunction
        {

            public override void Awake()
            {

            }

            public override void Func(ref line_c line, bool _isInLine)
            {
                if (this.scriptMaster.getVNHandler() == null || this.scriptMaster.getVNHandler().VNMode == false)
                    this.scriptMaster.TEXT.Reset(true, this.scriptMaster.NextLine);
                else
                    this.scriptMaster.NextLine(_isInLine);
            }
        }
    }
}