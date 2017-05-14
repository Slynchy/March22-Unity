using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartThrobObjectScript : MonoBehaviour
{

    public M22.ScriptMaster.VoidDelegate callback;

    Image IMG;

    int stage = 0;

	void Start ()
    {
        IMG = GetComponent<Image>();
        IMG.color = new Color(255, 255, 255, 0);
	}
	
	void Update ()
    {
        switch(stage)
        {
            case 0:
                IMG.color = new Color(255, 255, 255, Mathf.Lerp(IMG.color.a, 0.4f, Time.deltaTime * 4));
                if (IMG.color.a > 0.33f)
                    stage++;
                break;
            case 1:
                IMG.color = new Color(255, 255, 255, Mathf.Lerp(IMG.color.a, -0.07f, Time.deltaTime * 2));
                if (IMG.color.a <= 0)
                    stage++;
                break;
            case 2:
                callback();
                Destroy(this.gameObject);
                break;
        }
	}
}
