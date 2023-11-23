using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class MonsterScript : MonoBehaviour
{
    [SerializeField] private AudioClip[] footSounds;
    [SerializeField] private AudioClip screamSound;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float visionRange;
    [SerializeField] private float visionAngle;
    [SerializeField] private List<Transform> moveLocations;
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float runSpeed = 3.5f;

    private Transform currentLocation;
    private enum monsterStates {walking, running, searching, idle, screaming, hitting};
    private monsterStates currentState = monsterStates.idle;

    Animator animator;
    private AudioSource audioSource;
    public bool shouldRoar = false;
    public bool shouldWalk = false;
    public bool shouldRun = false;
    public bool shouldAttack = false;
    public bool shouldIdle = false;
    public bool shouldSearch = false;
    private List<GameObject> players;
    private ConeFieldOfView fieldOfView;
    private float timer = 3f;
    private bool playerInView = false; 
    private NavMeshAgent navMeshAgent;
    public bool shouldIMove = false;

    private Vector3 playerLastLocation;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        fieldOfView = GetComponent<ConeFieldOfView>();
    }


    private void Scream()
    {
        audioSource.PlayOneShot(screamSound);
    }


    // Start is called before the first frame update
    void Start()
    {
        //shouldWalk = true;
        //animator.SetBool("IsWalking", shouldWalk);
        currentState = monsterStates.walking;
        selectLocation();
    }

    // update is called once per frame
    void Update()
    {


        if (fieldOfView.FindVisableTargets())
        {
            currentState = monsterStates.running;
        }
        else
        {
            //currentState = monsterStates.walking;
        }

        switch (currentState)
        {
            case monsterStates.idle:
                {
                    shouldWalk = false;
                    shouldIdle = true;
                    shouldSearch = true;
                    setAllAnimations();
                    if (timer <= 0f)
                    {
                        shouldIMove = true;
                        selectLocation();
                        currentState = monsterStates.walking;
                    }
                    timer = timer - Time.deltaTime;
                    break;
                }
            case monsterStates.walking:
                {
                    timer = 3f;
                    //currentState= monsterStates.running;    
                    navMeshAgent.speed = walkSpeed;
                    shouldWalk = true;
                    shouldSearch = false;
                    shouldRun = false;
                    shouldIdle = false;
                    setAllAnimations();
                    navMeshAgent.SetDestination(currentLocation.position);
                    if(Vector3.Distance(transform.position, currentLocation.position) <= 1.0f)
                    {
                        shouldIMove = false;
                        currentState = monsterStates.idle;
                        
                    }
                    break;
                }
            case monsterStates.running:
                {
                    //navMeshAgent.speed = runSpeed;
                    shouldRun = true;
                    shouldWalk = false;
                    shouldIdle = false;
                    shouldAttack = false;
                    shouldRoar = false;
                    shouldSearch = false;

                    setAllAnimations();
                    navMeshAgent.enabled = false;
                    MoveTowardsPlayer();
                    if (!fieldOfView.FindVisableTargets())
                    {
                        currentState = monsterStates.searching;
                    }
                    break;
                }
            case monsterStates.screaming:
                {
                    break;
                }
            case monsterStates.hitting:
                {
                    break;
                }
            case monsterStates.searching:
                {
                    navMeshAgent.enabled = true;
                    navMeshAgent.Move( playerLastLocation);
                    shouldIdle = false;
                    shouldSearch = false;
                    shouldWalk = true;


                    

                    if (Vector3.Distance(transform.position, currentLocation.position) <= 1.0f)
                    {
                        shouldIdle = true;
                        shouldWalk = false;
                        shouldSearch = true;
                    }

                    setAllAnimations();
                    break;
                }
            default:
                {
                    break;
                }
        }


        players = GameObject.FindGameObjectsWithTag("Player").ToList<GameObject>();

        //if (navMeshAgent.velocity.magnitude > 0.1f)
        //{
        //    shouldWalk = true;
        //}




        Debug.Log("Distance to destination: " + Vector3.Distance(transform.position, currentLocation.position));

        //if(!shouldWalk && shouldRun)
        //{
        //    animator.SetBool("IsRunning", shouldRun);
        //    animator.SetBool("IsWalking", shouldWalk);
        //}
        //if(shouldWalk && !shouldRun)
        //{
        //    animator.SetBool("IsRunning", shouldRun);
        //    animator.SetBool("IsWalking", shouldWalk);
        //}
        //if (!shouldWalk && shouldRoar)
        //{
        //    animator.SetBool("IsRoaring", shouldRoar);
        //    animator.SetBool("IsWalking", shouldWalk);
        //    shouldRoar = false;
        //    if (timer >= 0)
        //    {
        //        timer = timer- Time.deltaTime;
        //    }
        //    else
        //    {

        //        shouldRun = true;
        //        timer = 1.5f;
        //    }


        //}
        //if (shouldWalk && !shouldRoar)
        //{
        //    animator.SetBool("IsRoaring", shouldRoar);
        //    animator.SetBool("IsWalking", shouldWalk);
        //}
        //if (!shouldRun && shouldRoar)
        //{
        //    animator.SetBool("IsRoaring", shouldRoar);
        //    animator.SetBool("IsRunning", shouldRun);
        //}
        //if (shouldRun && !shouldRoar)
        //{
        //    animator.SetBool("IsRoaring", shouldRoar);
        //    animator.SetBool("IsRunning", shouldRun);
        //    if (playerInView)
        //    {
        //        MoveTowardsPlayer();
        //    }
        //    else
        //    {
        //        shouldRun = false;
        //    }

        //}

        //if (!shouldWalk && shouldAttack)
        //{
        //    animator.SetBool("IsAttacking", shouldAttack);
        //    animator.SetBool("IsWalking", shouldWalk);
        //}
        //if (shouldWalk && !shouldAttack)
        //{
        //    animator.SetBool("IsAttacking", shouldAttack);
        //    animator.SetBool("IsWalking", shouldWalk);
        //}
        //if (!shouldRun && shouldAttack)
        //{
        //    animator.SetBool("IsAttacking", shouldAttack);
        //    animator.SetBool("IsRunning", shouldRun);
        //}
        //if (shouldRun && !shouldAttack)
        //{
        //    animator.SetBool("IsAttacking", shouldAttack);
        //    animator.SetBool("IsRunning", shouldRun);
        //}

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

    private void selectLocation()
    {
        Transform newLocation = moveLocations[UnityEngine.Random.Range(0, moveLocations.Count)];

        if (newLocation == currentLocation)
        {
            selectLocation();
        }

        currentLocation = newLocation;
    }

    private void setAllAnimations()
    {
        animator.SetBool("IsIdle", shouldIdle);
        animator.SetBool("IsWalking", shouldWalk);
        animator.SetBool("IsSearching", shouldSearch);
        animator.SetBool("IsRunning", shouldRun);
        animator.SetBool("IsRoaring", shouldRun);
        animator.SetBool("IsAttacking", shouldAttack);
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

    }

    void MoveTowardsPlayer()
    {
        if(fieldOfView.visablesTargets != null)
        {

            if (fieldOfView.visablesTargets[0])
            {
                Transform selectedTarget = fieldOfView.visablesTargets[0];

                if (selectedTarget != null)
                {

                    playerLastLocation = selectedTarget.position;
                    Vector3 directionToPlayer = selectedTarget.position - transform.position;
                    directionToPlayer.y = 0;
                    directionToPlayer.Normalize();
                    transform.forward = directionToPlayer;
                    transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
                }
            }
            
        }     
        
    }


}
