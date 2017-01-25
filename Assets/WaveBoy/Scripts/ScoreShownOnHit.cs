using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A script that lives on the short-lived score effect.
public class ScoreShownOnHit : MonoBehaviour {

    private float lifeTime = 3.0f;

    public void UpdateTextAndGo(string newText)
    {
        // Update the text based on the score!
        this.GetComponent<TextMesh>().text = newText;

        // We want to do a nice leantween animation up, and maybe play a sound, then wink out of existence
        LeanTween.moveLocalY(this.gameObject, 5.0f, lifeTime);
        LeanTween.scale(this.gameObject, Vector3.zero, lifeTime)
            .setEaseInOutElastic()
            .setOnComplete(() =>
            {
                GameObject.Destroy(this.gameObject);
            });
        
    }
}