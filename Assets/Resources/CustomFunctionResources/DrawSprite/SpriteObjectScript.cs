using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteObjectScript : MonoBehaviour {

    float speed = 1f;
    Image[] children;
    Text[] childrenText;
    RectTransform RT;
    float origYPos;
    bool entered = false;

    private void Awake()
    {
        RT = this.GetComponent<RectTransform>();
        origYPos = RT.anchoredPosition.y;
    }

    public IEnumerator Enter()
    {
        float progress = 0;
        while(progress < 1.0f)
        {
            progress = Mathf.Lerp(progress, 1.2f, Time.deltaTime * speed);
            foreach (var item in children)
            {
                var col = item.color;
                col.a = progress;
                item.color = col;
            }
            foreach (var item in childrenText)
            {
                var col = item.color;
                col.a = progress;
                item.color = col;
            }
            RT.anchoredPosition = new Vector2(0, Mathf.Lerp(origYPos, 0, progress));
            yield return null;
        }
        Camera.main.GetComponent<M22.ScriptMaster>().NextLine(false);
        entered = true;
    }

    public void Hide()
    {
        StartCoroutine(Exit());
    }

    public void SetSprite(Sprite _spr)
    {
        if(_spr != null)
            this.GetComponent<Image>().sprite = _spr;
    }

    IEnumerator Exit()
    {
        float progress = 0;
        while (progress < 1.0f)
        {
            progress = Mathf.Lerp(progress, 1.2f, Time.deltaTime * speed);
            foreach (var item in children)
            {
                var col = item.color;
                col.a = 1 - progress;
                item.color = col;
            }
            foreach (var item in childrenText)
            {
                var col = item.color;
                col.a = 1 - progress;
                item.color = col;
            }
            RT.anchoredPosition = new Vector2(0, Mathf.Lerp(0, origYPos, progress));
            yield return null;
        }
        Camera.main.GetComponent<M22.ScriptMaster>().NextLine(false);
        Destroy(this.gameObject);
    }

    void Start () {
        children = this.gameObject.GetComponentsInChildren<Image>();
        childrenText = this.gameObject.GetComponentsInChildren<Text>();
        foreach (var item in children)
        {
            var col = item.color;
            col.a = 0;
            item.color = col;
        }
        foreach (var item in childrenText)
        {
            var col = item.color;
            col.a = 0;
            item.color = col;
        }
        StartCoroutine(Enter());
	}
}
