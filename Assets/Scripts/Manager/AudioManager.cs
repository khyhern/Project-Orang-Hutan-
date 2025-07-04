using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgm;
    [SerializeField] private AudioSource _sfxWalking;
    [SerializeField] private AudioSource _sfxSeeEnemy;
    [SerializeField] private AudioSource _sfxHeartBeat;

    [Header("Audio Clips")]
    public AudioClip Bgm;
    public AudioClip Walking;
    public AudioClip HeartBeat;
    public AudioClip MurdererCome;

    private bool firstPlay = true;

    private void Start()
    {
        PlayBGM(Bgm);
        PlayHeartBeat();
    }

    private void PlayBGM(AudioClip clip)
    {
        _bgm.clip = clip;
        _bgm.loop = true;
        _bgm.Play();    
    }

    private void PlayHeartBeat()
    {
        _sfxHeartBeat.clip = HeartBeat;
        _sfxHeartBeat.Play();
    }

    public void PlaySFXseeEnemy()
    {
        if (!firstPlay) return;
        firstPlay = false;
        _sfxSeeEnemy.PlayOneShot(MurdererCome);
        StartCoroutine(SeeEnemyCountDown());
    }

    private IEnumerator SeeEnemyCountDown()
    {
        yield return new WaitForSeconds(60f);
        firstPlay = true;
    }

    public void PlaySFXWalk()
    {
        _sfxWalking.pitch = Random.Range(0.7f, 0.9f);
        _sfxWalking.PlayOneShot(Walking);
    }

    public void IncreaseVolumeHB()
    {
        if (_sfxHeartBeat.volume < 0.8f)
        {
            _sfxHeartBeat.volume += 0.1f * Time.deltaTime;
        }
    }

    public void DecreaseVolumeHB()
    {
        if (_sfxHeartBeat.volume >= 0.1f)
        {
            _sfxHeartBeat.volume -= 0.1f * Time.deltaTime;
        }
    }
}
