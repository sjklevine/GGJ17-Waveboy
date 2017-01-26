using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("pedal");
        GameManager.instance.NewGame();
        SceneManager.LoadScene("Town"); 
    }
}