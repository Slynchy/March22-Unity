using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M22
{

    public class InputWrapper
    {

        static public bool NextLineButton()
        {
            switch(Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WebGLPlayer:
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.LinuxPlayer:
                    return Input.GetKeyDown(KeyCode.Return);
                default:
                    return false;
            }
        }

        static public bool SkipTextButton()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WebGLPlayer:
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.LinuxPlayer:
                    return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                default:
                    return false;
            }
        }

    }

}