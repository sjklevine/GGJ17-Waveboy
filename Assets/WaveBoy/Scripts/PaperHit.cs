using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperHit : MonoBehaviour
{
    public int valueScore = 50;
    private bool didWeScoreyet = false;
    // Use this for initialization
    void Start()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!didWeScoreyet)
        {

            if (collision.gameObject.tag == "Paper")
            {
                GameManager.instance.score += valueScore;
                Destroy(this.gameObject);
            }

            else
            {
                GameManager.instance.score += valueScore / 5;
            }



        }

    }


}