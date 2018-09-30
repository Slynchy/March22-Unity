using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextboxScript : MonoBehaviour
{
    private Image img;
    private Text[]  txt;
    private Coroutine currImgCoroutine;
    private Coroutine[] currTxtCoroutines;

    // Use this for initialization
    void Start()
    {
        img = this.gameObject.GetComponent<Image>();
        txt = new Text[this.gameObject.transform.childCount];
        currImgCoroutine = null;
        currTxtCoroutines = new Coroutine[this.gameObject.transform.childCount];

        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            var obj = this.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>();
            txt[i] = obj;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator FadeInTXT(Text img, float FadeTime, int index)
    {
        float newAlpha = 0;

        while (newAlpha < 1)
        {
            newAlpha = newAlpha + (Time.deltaTime * FadeTime);

            img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);

            yield return null;
        }
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        currTxtCoroutines[index] = null;
    }

    public IEnumerator FadeOutTXT(Text img, float FadeTime, int index)
    {
        float newAlpha = 1;

        while (newAlpha > 0)
        {
            newAlpha = newAlpha - (Time.deltaTime * FadeTime);

            img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);

            yield return null;
        }
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        currTxtCoroutines[index] = null;
    }

    public IEnumerator FadeOutIMG(Image img, float FadeTime)
    {
        float newAlpha = 1;

        while (img.color.a > 0)
        {
            newAlpha = newAlpha - (Time.deltaTime * FadeTime);

            img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);

            yield return null;
        }
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        currImgCoroutine = null;
    }

    public IEnumerator FadeInIMG(Image img, float FadeTime)
    {
        float newAlpha = 0;

        while (img.color.a < 1)
        {
            newAlpha = newAlpha + (Time.deltaTime * FadeTime);

            img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);

            yield return null;
        }
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        currImgCoroutine = null;
    }

    public void HideText(bool _fade = true, float _speed = 6.0f)
    {
        for (int i = 0; i < txt.Length; i++)
        {
            if (_fade == true)
            {
                if (currTxtCoroutines[i] != null)
                {
                    Debug.Log("STOPPING CO ROUTINE");
                    StopCoroutine(currTxtCoroutines[i]);
                    txt[i].color = new Color(1, 1, 1, 1);
                    currTxtCoroutines[i] = null;
                }
                currTxtCoroutines[i] = StartCoroutine(FadeOutTXT(txt[i], _speed, i));
            }
            else
                txt[i].color = new Color(1, 1, 1, 0);
        }

        if (_fade == true)
        {
            if (currImgCoroutine != null)
            {
                Debug.Log("STOPPING CO ROUTINE");
                StopCoroutine(currImgCoroutine);
                img.color = new Color(1, 1, 1, 1);
                currImgCoroutine = null;
            }
            currImgCoroutine = StartCoroutine(FadeOutIMG(img, _speed));
        }
        else
            img.color = new Color(1, 1, 1, 0);
    }

    public void ShowText(bool _fade = true, float _speed = 6.0f)
    {
        for (int i = 0; i < txt.Length; i++)
        {
            if (_fade == true)
            {
                if(currTxtCoroutines[i] != null)
                {
                    StopCoroutine(currTxtCoroutines[i]);
                    txt[i].color = new Color(1, 1, 1, 0);
                    currTxtCoroutines[i] = null;
                }
                currTxtCoroutines[i] = StartCoroutine(FadeInTXT(txt[i], _speed, i));
            }
            else
                txt[i].color = new Color(1, 1, 1, 1);
        }

        if (_fade == true)
        {
            if (currImgCoroutine != null)
            {
                StopCoroutine(currImgCoroutine);
                img.color = new Color(1, 1, 1, 0);
                currImgCoroutine = null;
            }
            currImgCoroutine = StartCoroutine(FadeInIMG(img, _speed));
        }
        else
            img.color = new Color(1, 1, 1, 1);
    }
}
