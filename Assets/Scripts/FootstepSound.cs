using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FootstepSound : StateMachineBehaviour
{
    [SerializeField] List<AudioClip> footstepSounds;
    private AudioClip selectedSound;
    private AudioSource audioSource1;
    private AudioSource audioSource2;
    [SerializeField] private AudioLowPassFilter lowPassFilter;
    [SerializeField] private AudioHighPassFilter highPassFilter;
    [SerializeField] private AudioReverbFilter reverbFilter;
    float minDistance = 1.0f;      
    float maxDistance = 20f;    
    float rolloffFactor = 1.0f;    
    float reverbLevel = 0.1f;     
    float occlusionLevel = 0.5f;  
    float lowPassFilterCutoff = 500.0f;
    float highPassFilterCutoff = 50.0f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lowPassFilter = animator.GetComponent<AudioLowPassFilter>();
        highPassFilter = animator.gameObject.GetComponent<AudioHighPassFilter>();
        reverbFilter = animator.gameObject.gameObject.GetComponent<AudioReverbFilter>();

        if (audioSource1 != null)
        {
            
        }
        else
        {
            audioSource1 = animator.gameObject.AddComponent<AudioSource>();
            audioSource2 = animator.gameObject.AddComponent<AudioSource>();
        }

        audioSource1.volume = 0.2f;
        audioSource2.volume = 0.2f;
        audioSource1.spatialize = true; 
        audioSource2.spatialize = true;
        audioSource1.spatialBlend = 1.0f;
        audioSource2.spatialBlend = 1.0f;
        audioSource1.spread = 0; 
        audioSource2.spread = 0;

        audioSource1.minDistance = minDistance;
        audioSource2.minDistance = minDistance;
        audioSource1.maxDistance = maxDistance;
        audioSource2.maxDistance = maxDistance;
        audioSource1.rolloffMode = AudioRolloffMode.Linear;
        audioSource2.rolloffMode = AudioRolloffMode.Linear;

        
        lowPassFilter.cutoffFrequency = lowPassFilterCutoff;
        highPassFilter.cutoffFrequency = highPassFilterCutoff;


        //reverbFilter.reverbLevel = reverbLevel;
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
