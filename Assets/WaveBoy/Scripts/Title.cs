using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	

    private void OnCollisionEnter(Collision collision)
    {
       
        GameManager.instance.NewGame();
        SceneManager.LoadScene("Town"); 
    }
}