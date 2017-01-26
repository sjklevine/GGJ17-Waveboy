using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBoyTossableNewspaper : MonoBehaviour {

    public GameObject scorePrefab;
    private bool alreadyScored;

    // Called when the paper hits something!
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("ON COLLISION ENTER BABY");

        // Always play some audio!  Later, right now this is way too noisy.
        //this.GetComponent<AudioSource>().Play();

        // Only score once per collision; seems dumb, but prevents weird repeat behavior.
        if (!alreadyScored) { 
            // If you hit a good tag, pop the score prefab right here and inform the GameManager.
            int pointsScored = 0;
            switch (collision.gameObject.tag)
            {
                case "House":
                    pointsScored = 100;
                    break;
                case "Person":
                    pointsScored = 500;
                    
                    //Take this opportunity to make the person animate!
                    Animator personAnim = collision.gameObject.GetComponent<Animator>();
                    personAnim.SetTrigger("OnHit");
                    PeopleSpeach personAudio = collision.gameObject.GetComponent<PeopleSpeach>();
                    personAudio.PlayOnFall();
                    break;
                case "Mailbox":
                    pointsScored = 1000;
                    break;
                case "Grass":
                    pointsScored = 250;
                    break;
            }
            if (pointsScored > 0)
            {
                // Play the audio on points scored now!
                this.GetComponent<AudioSource>().Play();

                // Pop a nice score object!
                GameObject scoreObj = (GameObject) GameObject.Instantiate(scorePrefab, collision.transform.position, Camera.main.transform.rotation);
                ScoreShownOnHit scoreScript = scoreObj.GetComponent<ScoreShownOnHit>();
                scoreScript.UpdateTextAndGo("+" + pointsScored);

                // Tell the gamemanager!
                 GameManager.instance.score += pointsScored;

                // Stop this object from scoring again!
                // But don't delete it, we need that sound effect.
                alreadyScored = true;
            }
        }
    }
}