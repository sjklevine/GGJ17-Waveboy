using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("pedal");
        GameManager.instance.NewGame();
        SceneManager.LoadScene("Town"); 
    }
}