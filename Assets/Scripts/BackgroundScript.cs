using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M22
{

    public class BackgroundScript : MonoBehaviour
    {

        RectTransform RT;
        ScriptMaster SM;
        VNHandler VN;

        private IEnumerator MoveToPos(int _newXpos, int _newYPos)
        {
            Vector2 oldPos = RT.anchoredPosition;
            Vector2 newPos = new Vector2(_newXpos, _newYPos);
            float progress = 0.0f;
            float speed = VN.GetMovementSpeed();
            while (RT.anchoredPosition != newPos)
            {
                //progress += Time.deltaTime * 1.0f;
                progress = Mathf.Lerp(progress, 1.2f, Time.deltaTime * speed);
                RT.anchoredPosition = Vector2.Lerp(oldPos, newPos, progress);
                yield return null;
            }

            SM.FinishBackgroundMovement();
        }

        private void Awake()
        {
            RT = this.GetComponent<RectTransform>();
            RT.sizeDelta = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            RT.anchoredPosition = new Vector2(Screen.currentResolution.width * 0.5f, Screen.currentResolution.height * 0.5f);
        }

        public void UpdateBackground(int _xOff, int _yOff, float _w, float _h)
        {
            RT.sizeDelta = new Vector2(Screen.currentResolution.width * _w, Screen.currentResolution.height * _h);
            RT.anchoredPosition = new Vector2((RT.sizeDelta.x * 0.5f) + _xOff, (RT.sizeDelta.y * 0.5f) + _yOff);
        }

        public void UpdatePos(int _xOff, int _yOff)
        {
            StartCoroutine(MoveToPos(
                (int)((RT.sizeDelta.x * 0.5f) + _xOff), 
                (int)((RT.sizeDelta.y * 0.5f) + _yOff))
            );
        }

        // Use this for initialization
        void Start()
        {
            SM = Camera.main.GetComponent<ScriptMaster>();
            VN = Camera.main.GetComponent<VNHandler>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}