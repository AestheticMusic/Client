using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [HideInInspector]
    public float musicLength = 0f;
    public float musicTime
    {
        get
        {
            return musicSource.time;
        }
        set
        {
            musicSource.time = value;
        }
    }

    public float musicProgress
    {
        get
        {
            return Mathf.Clamp01(musicSource.time / musicSource.clip.length);
        }
    }

    private AudioSource musicSource;
    private AudioSource[] seSource = new AudioSource[2];
    private int seSourceIndex = 0;
    private GameManager g;


    private void Awake()
    {
        instance = this;

        AudioSource[] sources = this.GetComponents<AudioSource>();
        musicSource = sources[0];
        seSource[0] = sources[1];
        seSource[1] = sources[2];
    }

    private void Start()
    {
        g = GameManager.instance;
    }

    private void Update()
    {

        if (g.isPlay && musicProgress == 1)
        {
            g.isPlay = false;
        }

    }

    public void LoadMusic(AudioClip _music)
    {
        musicSource.clip = _music;
        musicLength = _music.length;
    }

    public void Play()
    {
        musicSource.Play();
    }

    public void Stop()
    {
        musicSource.Stop();
    }

    public void Pause()
    {
        musicSource.Pause();
    }

    public void UnPause()
    {
        musicSource.UnPause();
    }

    public void PlaySE(AudioClip _se, float _volume)
    {
        seSourceIndex = (seSourceIndex + 1) % 2;

        seSource[seSourceIndex].PlayOneShot(_se, _volume);
    }


}
