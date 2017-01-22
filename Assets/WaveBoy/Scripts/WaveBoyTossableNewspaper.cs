using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBoyTossableNewspaper : MonoBehaviour {

    public GameObject scorePrefab;

    // Called when the paper hits something!
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ON COLLISION ENTER BABY");

        // Always play some audio!
        this.GetComponent<AudioSource>().Play();

        // If you hit a good tag, pop the score prefab right here and inform the GameManager.
        if (collision.gameObject.tag == "Finish")
        {
            GameObject scoreObj = (GameObject) GameObject.Instantiate(scorePrefab, collision.transform.position, Camera.main.transform.rotation);
            ScoreShownOnHit scoreScript = scoreObj.GetComponent<ScoreShownOnHit>();
            scoreScript.UpdateTextAndGo("+100"); // Change depending on point value

            //TODO: tell gamemanager!
        }
    }
}