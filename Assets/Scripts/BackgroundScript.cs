using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScript : MonoBehaviour {

    public float alpha = 1.0f;
    private float lastAlpha;
    private Material mat;

    private void Awake()
    {
        lastAlpha = alpha;
    }

	// Use this for initialization
	void Start () {
        mat = this.GetComponent<Image>().material;
	}
	
	// Update is called once per frame
	void Update () {
		if(lastAlpha != alpha)
        {
            mat.SetFloat("_Alpha", alpha);
            lastAlpha = alpha;
        }
	}
}
