using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _spiderHitSFX;
    [SerializeField] private AudioClip _flyHitSFX;
    [SerializeField] private AudioClip _webFlingSFX;

    private void OnEnable()
    {
        Spider.OnSpiderDeath += OnSpiderDeath;
        Fly.OnFlyDeath += OnFlyDeath;
        WebPoint.OnWebFling += OnWebFling;
    }

    private void OnWebFling()
    {
        PlayAudioClip(_webFlingSFX);
    }

    private void OnFlyDeath()
    {
        PlayAudioClip(_flyHitSFX);
    }

    private void OnSpiderDeath()
    {
        PlayAudioClip(_spiderHitSFX);
    }

    void PlayAudioClip(AudioClip audioClip)
    {
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }

    private void OnDisable()
    {
        Spider.OnSpiderDeath -= OnSpiderDeath;
        Fly.OnFlyDeath -= OnFlyDeath;
        WebPoint.OnWebFling -= OnWebFling;
    }

}
