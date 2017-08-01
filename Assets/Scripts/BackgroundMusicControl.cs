using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicControl : MonoBehaviour {

    public AudioSource audioSource;

    public AudioClip musicTrack;
    public AudioClip bossMusicTrack;
    public AudioClip victoryMusicTrack;
    public AudioClip gameOverMusicTrack;

	public void playBossMusic()
    {
        audioSource.loop = true;
        audioSource.clip = bossMusicTrack;
        audioSource.Play();
    }

    public void playMusicTrack()
    {
        audioSource.loop = true;
        audioSource.clip = musicTrack;
        audioSource.Play();
    }

    public void playGameOverTrack()
    {
        audioSource.loop = false;
        audioSource.clip = gameOverMusicTrack;
        audioSource.Play();
    }

    public void playVictoryTrack()
    {
        audioSource.loop = false;
        audioSource.clip = victoryMusicTrack;
        audioSource.Play();
    }

    public void stopMusic()
    {
        audioSource.Stop();
    }
}
