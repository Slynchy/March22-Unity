﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M22
{
    public class BackgroundTransition : MonoBehaviour
    {

        public enum IN_OR_OUT
        {
            IN,
            OUT
        }
        public IN_OR_OUT inOrOut = IN_OR_OUT.IN;

        public float Speed = 1.0f;
        public float Delay = 1.0f;
        [HideInInspector]
        public Sprite srcSprite;
        [HideInInspector]
        public Sprite destSprite;
        [HideInInspector]
        public Sprite effect;
        private Image img;
        private float inc = -1f;
        private float currDelay = 0;
        public M22.ScriptMaster.VoidDelegate callback;

        // Use this for initialization
        void Start()
        {
            img = this.gameObject.GetComponent<Image>();
            img.material.SetTexture("_MainTex", destSprite.texture);
            img.material.SetTexture("_SecondaryTex", effect.texture);
            currDelay = 0;

            img.material.SetColor("_AmbientLighting", RenderSettings.ambientLight);

            img.material.SetFloat("_Progress", inc);
            if (inOrOut == IN_OR_OUT.IN)
                img.material.SetFloat("_InOrOut", 0);
            else
                img.material.SetFloat("_InOrOut", 1);

            img.material.SetFloat("_Alpha", 1);
        }

        // Update is called once per frame
        void Update()
        {
            if (inc >= 1f)
            {
                inc = 1f;
                currDelay += Time.deltaTime;
                if (currDelay >= Delay)
                {
                    callback.Invoke();
                    Destroy(this.gameObject);
                }
            }
            else
                inc += Time.deltaTime * Speed;

            img.material.SetFloat("_Progress", inc);
        }
    }
}