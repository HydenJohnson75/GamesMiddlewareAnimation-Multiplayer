using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;

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
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private AudioListener audioListener;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            audioListener.enabled = true;
            virtualCamera.Priority = 1;
        }
        else
        {
            virtualCamera.Priority = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner && IsLocalPlayer)
        {
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

            animator.SetFloat("X", currentHorizontal);
            animator.SetFloat("Y", currentVertical);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("IsRunning", true);
            }
            else
            {
                animator.SetBool("IsRunning", false);
            }
            moveCamera();

            if (canInteract)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    InteractServerRPC();
                }
            }
        }
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

        }
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
