using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M22
{
    public class DebugDisplay : MonoBehaviour
    {

        Text debugTxt;
        M22.ScriptMaster M22SM;
        const float update_interval = 0.33f; // third of a second
        float currTimer = 0;
        float deltaTime = 0.0f;

        // Use this for initialization
        void Start()
        {
            debugTxt = GetComponentInChildren<Text>();
            M22SM = Camera.main.GetComponent<M22.SceneManager>().ScriptMaster;
        }

        // Update is called once per frame
        void Update()
        {
            currTimer += Time.deltaTime;
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            if (currTimer > update_interval)
            {
                currTimer = 0;
                float fps = 1.0f / deltaTime;
                debugTxt.text = string.Format("Current Line - <b>{0}</b>\n\nFPS - <b>{1:0.}</b>", M22SM.CURRENT_LINE.m_origScriptPos, fps);
            }
        }
    }
}