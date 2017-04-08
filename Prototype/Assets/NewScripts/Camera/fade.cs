using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fade : MonoBehaviour {
    public float speed;
    float vignette;
    public bool fadeOut, fadeIn;
	// Use this for initialization
	void Start () {
        vignette = GetComponent<PostEffectControl>().vignette;
	}
	
	// Update is called once per frame
	void Update () {
        if (fadeOut)
        {
            FadeOut();
        }
        else if(fadeIn)
        {
            FadeIn();
        }
	}

    void FadeIn()
    {
        speed /= 1.001f;
        vignette -= (Time.deltaTime * speed);

        if(vignette <= 10)
        {
            speed = 20.0f;
        }

        if (vignette <= 0)
        {
            fadeIn = false;
            speed = 2;
            vignette = 0;
        }

        GetComponent<PostEffectControl>().vignette = vignette;
    }

    void FadeOut()
    {
        vignette += (Time.deltaTime * speed);
        speed *= 1.5f;
        GetComponent<PostEffectControl>().vignette = vignette;

        if(vignette >=80)
        {
            speed = 400f;
			vignette = 80.0f;
            fadeOut = false;
        }
    }
}
