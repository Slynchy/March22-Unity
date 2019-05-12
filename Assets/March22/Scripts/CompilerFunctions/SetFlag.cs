using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace M22
{
    namespace Functions
    {
        public class SetFlag : InternalFunction
        {

            public override void Awake()
            {

            }

            public override void Func(ref line_c line, bool _isInLine)
            {
                if (this.scriptMaster.SCRIPT_FLAGS.Contains(line.m_parameters_txt[0]))
                {
                    // Flag already set, ignore
                }
                else
                {
                    this.scriptMaster.SCRIPT_FLAGS.Add(line.m_parameters_txt[0]);
                }
                this.scriptMaster.NextLine(_isInLine);
            }
        }
    }
}