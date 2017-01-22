using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour {




    void OnTriggerEnter(Collision collision)
    {
        Debug.Log(collision);
        if (collision.gameObject.tag == "Player")
            StartCoroutine("EndingWait");

    }

    IEnumerator EndingWait()
    {
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(30f);
        GameManager.instance.GameOver();
        yield break;
    }
}
