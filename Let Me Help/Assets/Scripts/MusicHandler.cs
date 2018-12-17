using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour {

    public AudioSource[] music;
    public int counter = 0, max = 1;

    AudioSource currentMusic;

	// Use this for initialization
	void Start () {
        currentMusic = music[counter];
        currentMusic.Play();
        CounterUp();
	}
	
	// Update is called once per frame
	void Update () {
        if (!currentMusic.isPlaying)
        {
            currentMusic = music[counter];
            currentMusic.Play();
            CounterUp();
        }
	}

    /// <summary>
    /// Ups the music counter and resets it to 0 when it exceeded the max amount
    /// </summary>
    void CounterUp()
    {
        counter++;
        if(counter > max)
        {
            counter = 0;
        }
    }
}
