using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace M22
{
    namespace Functions
    {
        public class DrawBackground : InternalFunction
        {

            public override void Awake()
            {

            }

            public override void Func(ref line_c _line, bool _isInLine)
            {
                var background = this.scriptMaster.getBackground();
                var backgroundScript = this.scriptMaster.getBackgroundScript();
                var backgroundTrans = this.scriptMaster.getBackgroundTrans();
                var backgroundTransScript = this.scriptMaster.getBackgroundTransScript();
                var WaitQueue = this.scriptMaster.getWaitQueue();

                if (backgroundScript.IsMoving())
                    backgroundScript.SetIsMoving(false);
                backgroundTrans.sprite = M22.BackgroundMaster.GetBackground(_line.m_parameters_txt[0]);
                //RectTransform tempRT = backgroundTrans.gameObject.GetComponent<RectTransform>();
                //tempRT.offsetMax = new Vector2(backgroundTrans.sprite.texture.height, 0);

                if (backgroundTrans.sprite == background.sprite)
                {
                    //WaitState = WAIT_STATE.BACKGROUND_MOVING;
                    WaitQueue.Add(new M22.WaitObject(WAIT_STATE.BACKGROUND_MOVING));
                    backgroundScript.UpdatePos(
                        _line.m_parameters[0],
                        _line.m_parameters[1]
                    );
                }
                else
                {
                    backgroundTransScript.UpdateBackground(
                        _line.m_parameters[0],
                        _line.m_parameters[1],
                        float.Parse(_line.m_parameters_txt[1]),
                        float.Parse(_line.m_parameters_txt[2])
                    );
                    backgroundTrans.color = new Color(1, 1, 1, 0.001f);
                }

                if (_line.m_parameters[2] == 1)
                {
                    //WaitState = WAIT_STATE.NOT_WAITING;
                    if (WaitQueue.Count > 0)
                        WaitQueue.RemoveAt(0);
                    this.scriptMaster.NextLine(_isInLine);
                }
            }
        }
    }
}