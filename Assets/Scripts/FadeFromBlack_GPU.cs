using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeFromBlack_GPU : MonoBehaviour {

	private Image img;
	private float inc = 1f;
    private float delay = 0;

	// Use this for initialization
	void Start () {
        img = this.gameObject.GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (inc <= 0f)
        {
            delay += Time.deltaTime;
            if(delay >= 2.0f)
            {
                inc = 1f;
                delay = 0;
            }
        }
        inc -= Time.deltaTime;
        img.material.SetFloat("_Progress", inc);
	}
}
