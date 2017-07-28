using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicControl : MonoBehaviour {

    public AudioSource audioSource;

    public AudioClip musicTrack;
    public AudioClip bossMusicTrack;

	public void playBossMusic()
    {
        audioSource.clip = bossMusicTrack;
        audioSource.Play();
    }

    public void playMusicTrack()
    {
        audioSource.clip = musicTrack;
        audioSource.Play();
    }
}
