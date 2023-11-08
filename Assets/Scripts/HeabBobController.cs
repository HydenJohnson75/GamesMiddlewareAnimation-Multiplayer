using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeabBobController : MonoBehaviour
{
    [SerializeField, Range(0, 0.1f)] private float amplitude = 0.015f;
    [SerializeField, Range(0, 30)] private float frequency = 10.0f;

    [SerializeField] private Transform camera = null;
    [SerializeField] private Transform cameraHolder = null;

    private float toggleSpeed = 3.0f;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
