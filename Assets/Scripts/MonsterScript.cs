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
    private enum monsterStates {walking, running, searching, idle, screaming, hitting, looking};
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
    private float searchTimer = 5f;

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

        Debug.Log(currentState);
        players = GameObject.FindGameObjectsWithTag("Player").ToList<GameObject>();
        switch (currentState)
        {
            case monsterStates.idle:
                {
                    shouldWalk = false;
                    shouldIdle = true;
                    shouldSearch = false;
                    setAllAnimations();
                    if (timer <= 0f)
                    {
                        shouldIMove = true;
                        selectLocation();
                        navMeshAgent.isStopped = false;
                        currentState = monsterStates.walking;
                    }
                    timer = timer - Time.deltaTime;
                    break;
                }
            case monsterStates.walking:
                {
                    timer = 3f;
                    searchTimer = 5f;
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
                    navMeshAgent.isStopped = true;

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
                    navMeshAgent.isStopped = false;
                    fieldOfView.visionRadius = 20;
                    //navMeshAgent.enabled = true;
                    navMeshAgent.SetDestination(playerLastLocation);
                    shouldIdle = false;
                    shouldSearch = false;
                    shouldWalk = true;
                    shouldRun = false;
                    setAllAnimations();


                    if (Vector3.Distance(transform.position, playerLastLocation) <= 1.0f)
                    {
                        currentState = monsterStates.looking;
                    }

                    
                    break;
                }
            case monsterStates.looking:
                {
                    shouldWalk = false;
                    shouldIdle = true;
                    shouldSearch = true;
                    shouldAttack = false;
                    shouldRoar = false;
                    shouldRun = false;
                    setAllAnimations();

                    if(timer <= 0f)
                    {
                        currentState = monsterStates.walking;
                    }
                    timer = timer -Time.deltaTime;

                    break;
                }
            default:
                {
                    break;
                }
        }


        



       // Debug.Log("Distance to destination: " + Vector3.Distance(transform.position, currentLocation.position));

    }

    private void selectLocation()
    {
        Transform newLocation = moveLocations[UnityEngine.Random.Range(0, moveLocations.Count)];

        while (newLocation == currentLocation)
        {
            newLocation = moveLocations[UnityEngine.Random.Range(0, moveLocations.Count)];
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
            if (fieldOfView.visablesTargets.Count > 0)
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


}
