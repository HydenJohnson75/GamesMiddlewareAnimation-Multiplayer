using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float minYRotation = -80f; // Minimum vertical rotation.
    public float maxYRotation = 80f;
    private float currentRotationX = 0f;
    public float sensitivity = 2.0f;
    public Transform cameraTransform;
    [SerializeField] private GameObject parentBody;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        moveCamera();
    }

    private void moveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        currentRotationX -= mouseY * sensitivity;
        currentRotationX = Mathf.Clamp(currentRotationX, minYRotation, maxYRotation);


        cameraTransform.localRotation = Quaternion.Euler(currentRotationX, 0, 0);
        parentBody.transform.Rotate(Vector3.up * mouseX * sensitivity);
    }

}
