using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace M22
{
    namespace Functions
    {
        public class StopSFXLooped : InternalFunction
        {

            public override void Awake()
            {

            }

            public override void Func(ref line_c line, bool _isInLine)
            {
                GameObject loopSFXobj = GameObject.Find(line.m_parameters_txt[0]);
                if (loopSFXobj == null)
                {
                    Debug.LogErrorFormat("Failed to stop looping SFX \"{0}\" at line {1}; is not currently playing!", line.m_parameters_txt[0], line.m_origScriptPos);
                    this.scriptMaster.NextLine();
                    return;
                }
                SFXScript stopSFXloop = loopSFXobj.GetComponent<SFXScript>();
                stopSFXloop.Stop(line.m_parameters_txt[1]);
                this.scriptMaster.NextLine();
            }
        }
    }
}