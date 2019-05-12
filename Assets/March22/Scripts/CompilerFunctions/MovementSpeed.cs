using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace M22
{
    namespace Functions
    {
        public class MovementSpeed : InternalFunction
        {

            public override void Awake()
            {

            }

            public override void Func(ref line_c line, bool _isInLine)
            {
                this.scriptMaster.getVNHandler().SetMovementSpeed(float.Parse(line.m_parameters_txt[0]));
                this.scriptMaster.NextLine(_isInLine);
            }
        }
    }
}