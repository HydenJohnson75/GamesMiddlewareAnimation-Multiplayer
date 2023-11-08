using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class MonsterScript : MonoBehaviour
{
    [SerializeField] private AudioClip[] footSounds;
    [SerializeField] private AudioClip screamSound;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float visionRange;
    [SerializeField] private float visionAngle;

    Animator animator;
    private AudioSource audioSource;
    public bool shouldRoar = false;
    public bool shouldWalk = false;
    public bool shouldRun = false;
    public bool shouldAttack = false;
    private List<GameObject> players;
    private ConeFieldOfView fieldOfView;
    private float timer = 1.5f;
    private bool playerInView = false; 

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        fieldOfView = GetComponent<ConeFieldOfView>();
    }

    private void Step_Left()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private void Step_Right()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private void Run_Left()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private void Run_Right()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private void Scream()
    {
        audioSource.PlayOneShot(screamSound);
    }

    private AudioClip GetRandomClip()
    {
        return footSounds[UnityEngine.Random.Range(0, footSounds.Length)];
    }

    // Start is called before the first frame update
    void Start()
    {
        shouldWalk = true;
        animator.SetBool("IsWalking", shouldWalk);
    }

    // update is called once per frame
    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player").ToList<GameObject>();

        if (fieldOfView.FindVisableTargets())
        {
            shouldWalk = false;
            shouldRoar = true;
            playerInView = true;
        }
        else{
            playerInView = false;
            shouldWalk = true;
        }
        
        if(!shouldWalk && shouldRun)
        {
            animator.SetBool("IsRunning", shouldRun);
            animator.SetBool("IsWalking", shouldWalk);
        }
        if(shouldWalk && !shouldRun)
        {
            animator.SetBool("IsRunning", shouldRun);
            animator.SetBool("IsWalking", shouldWalk);
        }
        if (!shouldWalk && shouldRoar)
        {
            animator.SetBool("IsRoaring", shouldRoar);
            animator.SetBool("IsWalking", shouldWalk);
            shouldRoar = false;
            if (timer >= 0)
            {
                timer = timer- Time.deltaTime;
            }
            else
            {
                
                shouldRun = true;
                timer = 1.5f;
            }
            
            
        }
        if (shouldWalk && !shouldRoar)
        {
            animator.SetBool("IsRoaring", shouldRoar);
            animator.SetBool("IsWalking", shouldWalk);
        }
        if (!shouldRun && shouldRoar)
        {
            animator.SetBool("IsRoaring", shouldRoar);
            animator.SetBool("IsRunning", shouldRun);
        }
        if (shouldRun && !shouldRoar)
        {
            animator.SetBool("IsRoaring", shouldRoar);
            animator.SetBool("IsRunning", shouldRun);
            if (playerInView)
            {
                MoveTowardsPlayer();
            }
            else
            {
                shouldRun = false;
            }
               
        }

        if (!shouldWalk && shouldAttack)
        {
            animator.SetBool("IsAttacking", shouldAttack);
            animator.SetBool("IsWalking", shouldWalk);
        }
        if (shouldWalk && !shouldAttack)
        {
            animator.SetBool("IsAttacking", shouldAttack);
            animator.SetBool("IsWalking", shouldWalk);
        }
        if (!shouldRun && shouldAttack)
        {
            animator.SetBool("IsAttacking", shouldAttack);
            animator.SetBool("IsRunning", shouldRun);
        }
        if (shouldRun && !shouldAttack)
        {
            animator.SetBool("IsAttacking", shouldAttack);
            animator.SetBool("IsRunning", shouldRun);
        }

        //if (shouldroar)
        //{
        //    animator.setbool("ischasing", shouldroar);

        //    shouldrun = true;
        //}
        //if (shouldrun && !shouldroar)
        //{
        //    animator.setbool("isrunningfast", shouldrun);
        //    // add logic to move towards the player here.
        //    movetowardsplayer();
        //}
        //if (!shouldrun && !shouldroar)
        //{
        //    transform.translate(vector3.forward * currentspeed * time.deltatime);
        //}
    }

    bool DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, visionRange);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = hitCollider.transform.position - transform.position;
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                if (angleToPlayer < visionAngle * 0.5f)
                {
                    //shouldRoar = true;
                    return true;
                }
            }
        }
        return false;

        //shouldRun = false; // No player detected.
    }

    void MoveTowardsPlayer()
    {
        if(fieldOfView.visablesTargets != null)
        {
            Transform selectedTarget = fieldOfView.visablesTargets[0];

            if (selectedTarget != null)
            {
                Vector3 directionToPlayer = selectedTarget.position - transform.position;
                directionToPlayer.y = 0; // Ensure the monster doesn't fly upwards.
                directionToPlayer.Normalize();
                transform.forward = directionToPlayer;
                transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
            }
        }     
        
    }


}
