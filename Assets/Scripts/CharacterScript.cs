using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M22
{
    public class CharacterScript : MonoBehaviour
    {
        public enum STATES
        {
            IDLE,
            FADEIN,
            FADEOUT,
            CHANGING_SPRITE
        }

        public bool complete = false;
        public float speed = 0.025f;
        public float delay = 1.0f;
        public Sprite effect;
        public Sprite destSpr;
        private Sprite tempSpr;
        private Image img;
        private STATES state;
        private float inc = -0.1f;
        private RectTransform rect;
        private Material mat;

        void Awake()
        {
            img = this.GetComponent<Image>();
            rect = this.GetComponent<RectTransform>();
            mat = Instantiate<Material>(img.material) as Material;
            img.material = mat;
            img.material.SetFloat("_Alpha", 0);

            state = STATES.FADEIN;
        }

        void Start()
        {
            tempSpr = destSpr;
            img.material.SetTexture("_SecondaryTex", destSpr.texture);
            img.material.SetTexture("_MainTex", tempSpr.texture);

            if (effect == null)
            {
                effect = Resources.Load<Sprite>("Images/white") as Sprite;
            }
            img.material.SetTexture("_TertiaryTex", effect.texture);
            img.material.SetColor("_AmbientLighting", RenderSettings.ambientLight);
            img.material.SetFloat("_Progress", inc);
            img.material.SetFloat("_InOrOut", 0);

            rect.sizeDelta = destSpr.rect.size;
        }

        public void DestroyCharacter(bool _immediately)
        {
            if (_immediately == true)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                state = STATES.FADEOUT;
            }
        }

        public STATES GetState() { return state; }

        public void UpdateSprite(GameObject _prefab, Sprite _new)
        {
            state = STATES.CHANGING_SPRITE;
            img.sprite = null;
            tempSpr = destSpr;
            destSpr = _new;
            img.material.SetTexture("_MainTex", tempSpr.texture);
            img.material.SetTexture("_SecondaryTex", destSpr.texture);
            inc = -0.1f;
            img.material.SetFloat("_Progress", inc);
            rect.sizeDelta = destSpr.rect.size;
            //img.material.SetTexture("_SecondaryTex", destSpr.texture);
            //img.sprite = destSpr;
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case STATES.IDLE:
                    return;
                case STATES.FADEIN:
                    float currAlphaIn = img.material.GetFloat("_Alpha");
                    if (currAlphaIn < 1.0f)
                    {
                        img.material.SetFloat("_Alpha", currAlphaIn + (speed * Time.deltaTime));
                    }
                    else
                    {
                        img.material.SetFloat("_Alpha", 1);
                        state = STATES.IDLE;
                    }
                    break;
                case STATES.FADEOUT:
                    float currAlphaOut = img.material.GetFloat("_Alpha");
                    if (currAlphaOut > 0.0f)
                    {
                        img.material.SetFloat("_Alpha", currAlphaOut - (speed * Time.deltaTime));
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                    break;
                case STATES.CHANGING_SPRITE:
                    if (inc >= 1f)
                    {
                        //inc = -0.1f;
                        state = STATES.IDLE;
                        img.material.SetTexture("_MainTex", tempSpr.texture);
                        img.material.SetTexture("_SecondaryTex", destSpr.texture);
                        img.sprite = destSpr;
                        // tempSpr = destSpr;
                        // img.material.SetTexture("_MainTex", tempSpr.texture);
                        //img.material.SetTexture("_SecondaryTex", destSpr.texture);
                        //img.material.SetTexture("_MainTex", tempSpr.texture);
                        //img.sprite = destSpr;
                    }
                    else
                        inc += (speed * Time.deltaTime);

                    img.material.SetFloat("_Progress", inc);
                    break;
            }
        }
    }

}
