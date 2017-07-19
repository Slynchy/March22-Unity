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
            CHANGING_SPRITE,
            MOVING
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
        private VNHandler parent;
        public string CurrentSpriteName;

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

            parent = Camera.main.GetComponent<VNHandler>();
            if (parent == null)
                Debug.LogError("Failed to get VNHandler from main camera!");

            if (effect == null)
            {
                effect = Resources.Load<Sprite>("Images/white") as Sprite;
            }
            img.material.SetTexture("_TertiaryTex", effect.texture);
            img.material.SetColor("_AmbientLighting", RenderSettings.ambientLight);
            img.material.SetFloat("_Progress", inc);
            img.material.SetFloat("_InOrOut", 0);

            rect.sizeDelta = destSpr.rect.size;
            StartCoroutine(FadeIn());
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
                StartCoroutine(FadeOut());
            }
        }

        public STATES GetState() { return state; }

        private IEnumerator MoveToPos(int _newXpos, int _newYPos)
        {
            state = STATES.MOVING;
            Vector2 oldPos = rect.anchoredPosition;
            Vector2 newPos = new Vector2(_newXpos, _newYPos);
            float progress = 0.0f;
            float speed = parent.GetMovementSpeed();
            switch(ScriptMaster.ActiveAnimationType)
            {
                case ScriptCompiler.ANIMATION_TYPES.SMOOTH:
                    while (rect.anchoredPosition != newPos || progress <= 1.0f)
                    {
                        //progress += Time.deltaTime * 1.0f;
                        progress = Mathf.Lerp(progress, 1.2f, Time.deltaTime * speed);
                        rect.anchoredPosition = Vector2.Lerp(oldPos, newPos, progress);
                        yield return null;
                    }
                    break;
                case ScriptCompiler.ANIMATION_TYPES.LERP:
                    while (rect.anchoredPosition != newPos || progress >= 1.0f)
                    {
                        progress += Time.deltaTime * speed;
                        rect.anchoredPosition = Vector2.Lerp(oldPos, newPos, progress);
                        yield return null;
                    }
                    break;
            }

            rect.anchoredPosition = newPos;
            if (state == STATES.MOVING)
                state = STATES.IDLE;
            //SM.FinishBackgroundMovement();
        }

        private IEnumerator FadeOut()
        {
            state = STATES.FADEOUT;
            float currAlphaOut = img.material.GetFloat("_Alpha");
            while (currAlphaOut > 0.0f)
            {
                currAlphaOut = currAlphaOut - (speed * Time.deltaTime);
                img.material.SetFloat("_Alpha", currAlphaOut);
                yield return null;
            }
            Destroy(this.gameObject);
        }

        private IEnumerator FadeIn()
        {
            state = STATES.FADEIN;
            float currAlphaIn = img.material.GetFloat("_Alpha");
            while (currAlphaIn < 1.0f)
            {
                currAlphaIn = currAlphaIn + (speed * Time.deltaTime);
                img.material.SetFloat("_Alpha", currAlphaIn);
                yield return null;
            }
            img.material.SetFloat("_Alpha", 1);
            if(state == STATES.FADEIN)
                state = STATES.IDLE;
        }

        public void UpdateSprite(GameObject _prefab, Sprite _new, string _newName, Vector2 newOffset = default(Vector2))
        {
            state = STATES.CHANGING_SPRITE;
            if(_newName.Equals(CurrentSpriteName))
            {
                // same sprite; just moving
                StartCoroutine(
                    MoveToPos(
                        (int)(0 + newOffset.x),
                        (int)(0 + newOffset.y)
                    )
                );
            }
            else
            {
                img.sprite = null;
                tempSpr = destSpr;
                destSpr = _new;
                img.material.SetTexture("_MainTex", tempSpr.texture);
                img.material.SetTexture("_SecondaryTex", destSpr.texture);
                inc = -0.1f;
                img.material.SetFloat("_Progress", inc);
                rect.sizeDelta = destSpr.rect.size;

                int newxpos = (int)(0 + newOffset.x);
                if (newxpos == (int)(rect.anchoredPosition.x))
                {
                    //nothing
                }
                else
                {
                    StartCoroutine(
                        MoveToPos(
                            (int)(0 + newOffset.x),
                            (int)(0 + newOffset.y)
                        )
                    );
                }

                //img.material.SetTexture("_SecondaryTex", destSpr.texture);
                //img.sprite = destSpr;
            }
            CurrentSpriteName = _newName;
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case STATES.IDLE:
                    return;
                case STATES.FADEIN:
                    break;
                case STATES.FADEOUT:
                    //float currAlphaOut = img.material.GetFloat("_Alpha");
                    //if (currAlphaOut > 0.0f)
                    //{
                    //    img.material.SetFloat("_Alpha", currAlphaOut - (speed * Time.deltaTime));
                    //}
                    //else
                    //{
                    //    Destroy(this.gameObject);
                    //}
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
