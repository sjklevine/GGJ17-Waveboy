using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorehit : MonoBehaviour {

    public int valueScore = 100;
	// Use this for initialization
	void Start () {
		
	}


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Paper")
        {
            GameManager.instance.score += valueScore;
            Destroy(this.gameObject);
        }

    }
}
