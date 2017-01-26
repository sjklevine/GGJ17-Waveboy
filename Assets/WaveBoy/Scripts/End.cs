using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour {

    public GameObject player;
    public TextMesh scoreText;
    public GameObject scoreHolder;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") { 
            StartCoroutine("EndingWait");
        }
    }

    IEnumerator EndingWait()
    {
        player.GetComponent<AudioSource>().Stop();
        scoreHolder.SetActive(true);
        scoreText.text = "Score: " + GameManager.instance.score;
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(30f);
        GameManager.instance.GameOver();
        yield break;
    }
}
