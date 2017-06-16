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
        private Image img;
        private STATES state;

        void Awake()
        {
            img = this.GetComponent<Image>();
            img.color = new Color(255, 255, 255, 0);
            state = STATES.FADEIN;
        }

        public void DestroyCharacter(bool _immediately)
        {
            if(_immediately == true)
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
            state = STATES.FADEOUT;
            GameObject tempGO = GameObject.Instantiate<GameObject>(_prefab, GameObject.Find("Characters").transform);
            tempGO.name = this.gameObject.name; //+ "-" + _modifier;
            tempGO.GetComponent<Image>().sprite = _new;
            tempGO.GetComponent<RectTransform>().offsetMin = this.GetComponent<RectTransform>().offsetMin;
        }
	
	    // Update is called once per frame
	    void Update ()
        {
            switch(state)
            {
                case STATES.IDLE:
                    return;
                case STATES.FADEIN:
                    if (img.color.a < 1.0f)
                    {
                        img.color = new Color(
                            1,
                            1,
                            1,
                            img.color.a + (speed * 2.0f)
                        );
                    }
                    else
                    {
                        img.color = new Color(
                            1,
                            1,
                            1,
                            1
                        );
                        state = STATES.IDLE;
                    }
                    break;
                case STATES.FADEOUT:
                    if (img.color.a > 0.0f)
                    {
                        img.color = new Color(
                            1,
                            1,
                            1,
                            img.color.a - (speed * 1.0f)
                        );
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                    break;
                case STATES.CHANGING_SPRITE:

                    break;
            }
        }
    }

}
