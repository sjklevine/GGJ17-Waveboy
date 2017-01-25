using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour {

    public GameObject player;


    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (other.gameObject.tag == "end")
            StartCoroutine("EndingWait");

    }

    IEnumerator EndingWait()
    {
        player.GetComponent<AudioSource>().Stop();
        GameManager.instance.scoreHolder.SetActive(true);
        GameManager.instance.scoreText.text = "Score: " + GameManager.instance.score;
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(10f);
        GameManager.instance.GameOver();
        yield break;
    }
}
