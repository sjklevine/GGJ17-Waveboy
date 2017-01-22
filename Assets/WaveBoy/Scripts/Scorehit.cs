using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorehit : MonoBehaviour {

    public int valueScore = 500;
    // Use this for initialization
    void Start () {
		
	}


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Paper")
        {
            GameManager.instance.score += valueScore;
            Destroy(this.gameObject);
        }

    }
}
