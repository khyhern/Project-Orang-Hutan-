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
    [SerializeField] private AudioSource _sfxGeneral;
    [SerializeField] private AudioSource _sfxBreath;
    [SerializeField] private AudioSource _sfxBlood;
    [SerializeField] private AudioSource _sfxEnemyFootstep;

    [Header("Audio Clips")]
    public AudioClip Bgm;
    public AudioClip Walking;
    public AudioClip HeartBeat;
    public AudioClip MurdererCome;
    public AudioClip MurdererAttack;
    public AudioClip Breath;
    public AudioClip Blood;
    public AudioClip EnemyFootstep;
    public AudioClip ScaryLaugh;

    private bool firstPlay = true;

    private void Start()
    {
        PlayBGM();
        PlayHeartBeat();
        _sfxEnemyFootstep = GameObject.Find("SFX - EnemyWalk").GetComponent<AudioSource>();
    }

    private void PlayBGM()
    {
        _bgm.clip = Bgm;
        _bgm.loop = true;
        _bgm.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        _sfxGeneral.PlayOneShot(clip);
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
        if (_sfxHeartBeat.volume < 0.7f)
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

    public void PlaySFXBreath()
    {
        _sfxBreath.PlayOneShot(Breath);
    }

    public void PlaySFXBlood()
    {
        _sfxBlood.PlayOneShot(Blood);
    }

    public void PlayEnemyFootstep()
    {
        _sfxEnemyFootstep.pitch = Random.Range(0.9f, 1.3f);
        _sfxEnemyFootstep.PlayOneShot(EnemyFootstep);
    }
}
