using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleSpeach : MonoBehaviour {
    AudioSource audioPlayer;
    public AudioClip[] voice; 
    public float lowAudio = 0.06f;
    public float highAudio = 0.15f;
 
    // Use this for initialization
    void Start () {
        audioPlayer = GetComponent<AudioSource>();
    }
	
	public void PlayVoice()
    {
        float vol = Random.Range(lowAudio, highAudio);
        audioPlayer.clip = voice[Random.Range(0, voice.Length)];
        audioPlayer.PlayOneShot(audioPlayer.clip, vol);
    }
}
