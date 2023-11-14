using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using System.Runtime.CompilerServices;
using UnityEngine.Animations.Rigging;

public class PlayerScript : NetworkBehaviour
{
    public float moveSpeed;
    public float sprintMultiplier;
    public float currentSpeed;
    public float parameterSmoothing = 5f;
    private Animator animator;
    public Transform character;
    public float minYRotation = -80f; // Minimum vertical rotation.
    public float maxYRotation = 80f;
    private float currentRotationX = 0f;
    public float sensitivity = 2.0f;
    public Transform cameraTransform;
    public float smoothing = 2.0f;
    private float currentHorizontal = 0f;
    private float currentVertical = 0f;
    private Door interactableObject;
    private bool canInteract = false;
    public bool isWalking;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private AudioSource audioSourceLFoot;
    [SerializeField] private AudioSource audioSourceRFoot;
    [SerializeField] private List<GameObject> meshesToDisable = new List<GameObject>();
    [SerializeField] private AudioClip footStepClip;
    [SerializeField] private GameObject aimingTarget;

    [SerializeField] private GameObject handBone;
    [SerializeField] private Transform flashLightPref;
    private Flashlight flashLight;
    [SerializeField] private Transform lFoot;
    [SerializeField] private Transform rFoot;
    [SerializeField] Rig R_HandIK;
    private bool isGroundedLeft;
    private bool isGroundedRight;

    Transform spawnedFlashlightTransform;

    NetworkVariable<int> IKRigWeight = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // Start is called before the first frame update
    void Start()
    {
        



        if (IsOwner)
        {
            animator = GetComponent<Animator>();

            

            //cameraTransform.GetComponentInChildren<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Head"));
            foreach (GameObject go in meshesToDisable)
            {
                go.SetActive(false);
            }

            

        }


        //else
        //{
        //    // Enable rendering of the head for other players' cameras
        //    cameraTransform.GetComponentInChildren<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("Head");
        //}
    }

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            audioListener.enabled = true;
            virtualCamera.Priority = 1;
            IKRigWeight = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
            if (aimingTarget != null)
            {
                aimingTarget.transform.position = virtualCamera.transform.position + virtualCamera.transform.forward;
            }
            else
            {
                return;
            }

            IKRigWeight.OnValueChanged += (oldVal, newVal) =>
            {
                R_HandIK.weight = newVal;
            };
        }
        else
        {
            virtualCamera.Priority = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if (aimingTarget != null)
            {
                aimingTarget.transform.position = virtualCamera.transform.position + virtualCamera.transform.forward;
            }
            else
            {
                return;
            }

            float targetHorizontal = Input.GetAxis("Horizontal");
            float targetVertical = Input.GetAxis("Vertical");

            currentHorizontal = Mathf.Lerp(currentHorizontal, targetHorizontal, Time.deltaTime * parameterSmoothing);
            currentVertical = Mathf.Lerp(currentVertical, targetVertical, Time.deltaTime * parameterSmoothing);

            Vector3 cameraForward = cameraTransform.forward;

            cameraForward.y = 0;
            cameraForward.Normalize();

            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : moveSpeed;

            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
            }


            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);

            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

            }


            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);
            }
            
            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.D))
            {
                isWalking = true;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    animator.SetBool("IsRunning", true);
                }
                else
                {
                    animator.SetBool("IsRunning", false);
                }
            }
            else
            {
                isWalking = false;
                animator.SetBool("IsRunning", false);
            }

            animator.SetFloat("X", currentHorizontal);
            animator.SetFloat("Y", currentVertical);


            moveCamera();

            if (canInteract)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    InteractServerRPC();
                }
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                SpawnFlashLightServerRpc();
            }

            if (spawnedFlashlightTransform != null)
            {
                MoveFlashlightClientRPC();
            }

            if(isWalking)
            {
                isGroundedLeft = Physics.Raycast(lFoot.transform.position, Vector3.down, 0.07f, LayerMask.GetMask("Floor"));
                isGroundedRight = Physics.Raycast(rFoot.transform.position, Vector3.down, 0.07f, LayerMask.GetMask("Floor"));

                Debug.DrawRay(lFoot.transform.position, Vector3.down * 0.07f, Color.cyan, 0.1f);
                Debug.DrawRay(rFoot.transform.position, Vector3.down * 0.07f, Color.cyan, 0.1f);

                if (isGroundedLeft == true)
                {
                    if (!audioSourceLFoot.isPlaying)
                    {
                        audioSourceLFoot.pitch = Random.Range(0.75f, 1.2f);
                        audioSourceLFoot.PlayOneShot(footStepClip);
                    }

                }

                if (isGroundedRight == true)
                {
                    if (!audioSourceRFoot.isPlaying)
                    {
                        audioSourceRFoot.pitch = Random.Range(0.75f, 1.2f);
                        audioSourceRFoot.PlayOneShot(footStepClip);
                    }

                }
            }


            if (Input.GetKeyDown(KeyCode.I)) 
            {
                R_HandIK.weight = R_HandIK.weight == 0 ? 1 : 0;
            }


            Debug.Log(isGroundedLeft);
        }

    }

    [ClientRpc] 
    private void moveClientRpc()
    {
        
    }

    private void moveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        currentRotationX -= mouseY * sensitivity;
        currentRotationX = Mathf.Clamp(currentRotationX, minYRotation, maxYRotation);

        cameraTransform.localRotation = Quaternion.Euler(currentRotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX * sensitivity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Floor")
        {
            if (other.gameObject.tag == "Interactable")
            {
                Debug.Log("Colliding");
                Door interactableDoor = other.gameObject.ConvertTo<Door>();
                interactableObject = interactableDoor;
                canInteract = true;
            }

            if (other.gameObject.tag == "Flashlight")
            {
                //flashLight = other.gameObject.ConvertTo<Flashlight>();
                //flashLight.pickUp();
                //flashLight.transform.SetParent(gameObject.transform,false);
            }

        }

        
    }


    [ServerRpc]
    private void SpawnFlashLightServerRpc()
    {
        spawnedFlashlightTransform = Instantiate(flashLightPref);
        spawnedFlashlightTransform.GetComponent<NetworkObject>().Spawn(true);
        spawnedFlashlightTransform.GetComponent<NetworkObject>().TrySetParent(gameObject,false);
        spawnedFlashlightTransform.position = handBone.transform.position;
    }

    [ClientRpc]
    private void MoveFlashlightClientRPC()
    {
        spawnedFlashlightTransform.position = handBone.transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        canInteract = false;
    }

    [ServerRpc]
    private void InteractServerRPC()
    {

        interactableObject.Interact();
    }
}
