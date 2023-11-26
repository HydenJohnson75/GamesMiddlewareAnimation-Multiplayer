using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSound : StateMachineBehaviour
{
    [SerializeField] List<AudioClip> footstepSounds;
    private AudioClip selectedSound;
    private AudioSource audioSource1;
    private AudioSource audioSource2;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSource1 = animator.gameObject.AddComponent<AudioSource>();
        audioSource2 = animator.gameObject.AddComponent<AudioSource>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float frame17Start = 0.1f;
        float frame17End = 0.3f;
        float frame25Start = 0.6f;
        float frame25End = 0.99f;

        float normalizedTime = stateInfo.normalizedTime % 1.0f;

        if (normalizedTime >= frame17Start && normalizedTime <= frame17End)
        {
            if (!audioSource1.isPlaying)
            {
                PlayFootstepSound(audioSource1);
            }
        }


        if (normalizedTime >= frame25Start && normalizedTime <= frame25End)
        {
            if (!audioSource2.isPlaying)
            {
                PlayFootstepSound(audioSource2);
            }
        }

        //Debug.Log(normalizedTime);
    }

    private void PlayFootstepSound(AudioSource audioSource)
    {
        selectedSound = footstepSounds[UnityEngine.Random.Range(0, footstepSounds.Count - 1)];

        if (selectedSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(selectedSound);
        }
    }
}
