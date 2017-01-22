using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour {




    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (other.gameObject.tag == "Player")
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
