using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleSpeach : MonoBehaviour {

    public AudioClip[] voice;
    public AudioClip onFallClip;
    public float lowAudio = 0.06f;
    public float highAudio = 0.15f;
    private AudioSource audioPlayer;
    
    void Start () {
        audioPlayer = GetComponent<AudioSource>();
    }
	
	public void PlayVoice()
    {
        // Vary the voice volume a bit for a little extra randomness...?
        float vol = Random.Range(lowAudio, highAudio);

        // Pick a random clip from your selection.
        audioPlayer.clip = voice[Random.Range(0, voice.Length)];

        // Play it!
        audioPlayer.PlayOneShot(audioPlayer.clip, vol);
    }

    public void PlayOnFall()
    {
        audioPlayer.clip = onFallClip;
        audioPlayer.PlayOneShot(audioPlayer.clip, 1.0f);
    }
}
