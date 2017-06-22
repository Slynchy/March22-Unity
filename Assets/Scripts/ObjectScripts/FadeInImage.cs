using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInImage : MonoBehaviour {

    public bool complete = false;
    public float speed = 0.025f;
    public float delay = 1.0f;
    private Image img;

	// Use this for initialization
	void Start () {
        img = this.GetComponent<Image>();
        img.color = new Color(255, 255, 255, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (complete) return;
		if(img.color.a < 1.0f)
        {
            img.color = new Color(
                255,
                255,
                255,
                img.color.a+speed
            );
        }
        else
        {
            img.color = new Color(
                255,
                255,
                255,
                img.color.a + Time.deltaTime
            );
            if(img.color.a >= (1.0f + delay))
                complete = true;
        }
	}
}
