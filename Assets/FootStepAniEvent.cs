using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepAniEvent : MonoBehaviour
{
    [SerializeField] List<AudioClip> footStepSounds = new List<AudioClip>();

    [SerializeField] AudioSource footSource;

    private void StepLeft()
    {
        //PlayRandomFootSound();
    }

    private void StepRight()
    {
        //PlayRandomFootSound();
    }

    private void PlayRandomFootSound()
    {
        AudioClip footClip = GetRandomSound();
        footSource.PlayOneShot(footClip);
    }

    private AudioClip GetRandomSound()
    {
        return footStepSounds[0];
    }

}
