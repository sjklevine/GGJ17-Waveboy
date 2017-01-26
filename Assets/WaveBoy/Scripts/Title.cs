using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {
    public GameObject player;
    public GameObject titleText;
    public GameObject instructionsText;

    private void Start()
    {
        titleText.transform.localScale = Vector3.zero;
        instructionsText.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("pedal");
        GameManager.instance.NewGame();
        SceneManager.LoadScene("Town"); 
    } 

    public void StartTitleSequence()
    {
        StartCoroutine(TitleSequence());
    }

    private IEnumerator TitleSequence()
    {
        // Play the audio
        player.GetComponent<AudioSource>().Play();

        // After x seconds, tween in the title
        yield return new WaitForSeconds(1.5f);
        LeanTween.scale(titleText, Vector3.one, 1.25f)
            .setEaseOutBounce();

        // After x further seconds, enable the "pedal to start instructions"
        yield return new WaitForSeconds(2.0f);
        instructionsText.SetActive(true);
    }
}