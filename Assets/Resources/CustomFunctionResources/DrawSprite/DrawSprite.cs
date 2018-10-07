using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M22
{
    namespace CustomFunctions
    {
        public class DrawSprite : CustomFunction
        {
            GameObject SpriteInstance;
            GameObject SpritePrefab;

            public override string Keyword()
            {
                return "DrawSprite";
            }

            public override void Awake()
            {
                SpritePrefab = Resources.Load<GameObject>("March22/CustomFunctionResources/DrawSprite/SpritePrefab") as GameObject;
            }

            public override void Func(string[] _params)
            {
                if (_params[0].Equals("out"))
                {
                    if (SpriteInstance == null)
                    {
                        UnityWrapper.LogWarningFormat("DrawSprite is null; trying to fade out when there isn't a sprite?");
                    }
                    else
                    {
                        SpriteInstance.GetComponent<SpriteObjectScript>().Hide();
                        SpriteInstance = null;
                    }
                }
                else
                {
                    SpriteInstance = GameObject.Instantiate<GameObject>(SpritePrefab, this.scriptMaster.GetCanvas(M22.ScriptMaster.CANVAS_TYPES.POSTCHARACTER).transform);
                    SpriteInstance.GetComponent<SpriteObjectScript>().SetSprite(Resources.Load<Sprite>("Images/" + _params[1]));
                }
            }
        }
    }
}