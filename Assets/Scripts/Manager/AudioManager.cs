using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgm;
    [SerializeField] private AudioSource _sfxWalking;
    [SerializeField] private AudioSource _sfx;
    [SerializeField] private AudioSource _sfxHeartBeat;

    [Header("Audio Clips")]
    public AudioClip Bgm;
    public AudioClip Walking;
    public AudioClip HeartBeat;
    public AudioClip MurdererCome;

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

    public void PlaySFX(AudioClip clip)
    {
        _sfx.PlayOneShot(clip);
    }

    public void PlaySFXWalk()
    {
        _sfxWalking.pitch = Random.Range(0.7f, 0.9f);
        _sfxWalking.PlayOneShot(Walking);
    }

    public void PlaySFXHearBeat()
    {
        if (_sfxHeartBeat.isPlaying) return;
        _sfxHeartBeat.clip = HeartBeat;
        _sfxHeartBeat.loop = true;
        _sfxHeartBeat.Play();
    }

    public void StopSFXHearBeat()
    {
        _sfxHeartBeat.Stop();
    }
}
