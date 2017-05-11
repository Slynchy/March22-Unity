using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutImage : MonoBehaviour {

    private bool doIt = false;
    public bool complete = false;
    public bool destroyWhenComplete = true;
    public float speed = 0.025f;
    public float delay = 1.0f;
    private Image img;

    // Use this for initialization
    void Start()
    {
        img = this.GetComponent<Image>();
    }

    public void FadeOut()
    {
        doIt = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (complete || doIt == false) return;
        if (img.color.a > 0.0f)
        {
            img.color = new Color(
                255,
                255,
                255,
                img.color.a - speed
            );
        }
        else
        {
            img.color = new Color(
                255,
                255,
                255,
                img.color.a - Time.deltaTime
            );
            if (img.color.a <= (0.0f - delay))
            {
                complete = true;
                if(destroyWhenComplete)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
