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
    private
    float minDistance = 1.0f;      
    float maxDistance = 20.0f;    
    float rolloffFactor = 1.0f;    
    float reverbLevel = 0.2f;     
    float occlusionLevel = 0.5f;  
    float lowPassFilterCutoff = 500.0f;
    float highPassFilterCutoff = 50.0f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSource1 = animator.gameObject.AddComponent<AudioSource>();
        audioSource2 = animator.gameObject.AddComponent<AudioSource>();
        audioSource1.volume = 0.2f;
        audioSource2.volume = 0.2f;

        audioSource1.minDistance = minDistance;
        audioSource2.minDistance = minDistance;
        audioSource1.maxDistance = maxDistance;
        audioSource2.maxDistance = maxDistance;
        audioSource1.rolloffMode = AudioRolloffMode.Linear;
        audioSource2.rolloffMode = AudioRolloffMode.Linear;
        


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
