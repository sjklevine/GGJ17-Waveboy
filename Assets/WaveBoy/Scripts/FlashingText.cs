using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingText : MonoBehaviour {

    public float flashPeriod = 1.0f;
    public Renderer text;
    private float timer;

	void Start () {
        timer = flashPeriod;		
	}
	
	void Update () {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            text.enabled = !text.enabled;
            timer = flashPeriod;
        }
    }
}
