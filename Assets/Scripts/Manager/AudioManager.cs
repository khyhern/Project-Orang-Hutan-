using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgm;
    [SerializeField] private AudioSource _sfxWalking;
    [SerializeField] private AudioSource _sfx;

    [Header("Audio Clips")]
    public AudioClip Bgm;
    public AudioClip Walking;

    private void Start()
    {
        PlayBGM(Bgm);
    }

    private void PlayBGM(AudioClip clip)
    {
        _bgm.clip = clip;
        _bgm.loop = true;
        _bgm.Play();    
    }

    public void PlaySFXWalk(AudioClip clip)
    {
        Debug.Log("Walk");
        _sfxWalking.clip = clip;
        _sfxWalking.Play();
    }
}
