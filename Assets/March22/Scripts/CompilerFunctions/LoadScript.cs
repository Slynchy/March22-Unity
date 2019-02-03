using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace M22
{
    namespace Functions
    {
        public class LoadScript : InternalFunction
        {

            public override void Awake()
            {

            }

            public override void Func(ref line_c line, bool _isInLine)
            {
                this.scriptMaster.LoadScript(line.m_parameters_txt[0]);
            }
        }
    }
}