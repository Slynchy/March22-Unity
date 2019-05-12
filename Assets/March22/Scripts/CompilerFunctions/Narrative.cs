using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace M22
{
    namespace Functions
    {
        public class Narrative : InternalFunction
        {

            public override void Awake()
            {

            }

            public override void Func(ref line_c line, bool _isInLine)
            {
                this.scriptMaster.TEXT.SetNewCurrentLine(line.m_lineContents);
                this.scriptMaster.getVNHandler().ClearCharacterName();
            }
        }
    }
}