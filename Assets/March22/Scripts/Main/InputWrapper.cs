using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M22
{

    public class InputWrapper
    {
        static bool stillActiveFromPreviousFrame = false;

        static public bool NextLineButton()
        {
            switch(Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WebGLPlayer:
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.LinuxPlayer:
                //case RuntimePlatform.OSXDashboardPlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return Input.GetKeyDown(KeyCode.Return);
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.Switch:
                case RuntimePlatform.PSP2:
                    if (Input.touchCount > 0)
                    {
                        var temp = Input.GetTouch(0).position;
                        if(temp.x > Screen.width * 0.3f && temp.x < (Screen.width - (Screen.width * 0.3f)))
                        {
                            if (temp.y > Screen.height * 0.3f && temp.y < (Screen.height - (Screen.height * 0.3f)))
                            {
                                if(stillActiveFromPreviousFrame == false)
                                {
                                    stillActiveFromPreviousFrame = true;
                                    return true;
                                }
                                    return false;
                            }
                            else
                            {
                                stillActiveFromPreviousFrame = false;
                                return false;
                            }
                        }
                        else
                        {
                            stillActiveFromPreviousFrame = false;
                            return false;
                        }
                    }
                    else
                    {
                        stillActiveFromPreviousFrame = false;
                        return false;
                    }
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
                //case RuntimePlatform.OSXDashboardPlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.Switch:
                case RuntimePlatform.PSP2:
                    if (Input.touchCount == 2)
                    {
                        return true;
                    }
                    else
                        return false;
                default:
                    return false;
            }
        }

    }

}