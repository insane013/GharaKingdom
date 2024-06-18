using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UnitAudio : MonoBehaviour
{
    [SerializeField] AudioClip _attackAudio;

    AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void AttackAnimationStartedProcessing(float delay)
    {
        StartCoroutine(PlayAudioWithDelay(delay));
    }

    IEnumerator PlayAudioWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _audioSource.clip = _attackAudio;
        _audioSource.Play();
    }
}
