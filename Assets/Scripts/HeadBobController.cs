using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Steamworks.InventoryItem;

public class HeadBobController : MonoBehaviour
{


    [SerializeField, Range(0.001f, 0.01f)]
    float amount = 0.002f;

    [SerializeField, Range(1f, 30f)]
    float frequency = 10.0f;

    [SerializeField, Range(10f, 100f)]
    float smoothing = 10.0f;

    Vector3 startPos;

    [SerializeField] Transform camTrans;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = camTrans.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        HeadBobTrigger();
        StopHeadBob();
    }

    private void HeadBobTrigger()
    {
        float inputMagnitude = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude;

        if(inputMagnitude > 0 )
        {
            StartHeadBob();
        }
    }

    private Vector3 StartHeadBob()
    {
        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency) * amount * 1.4f, smoothing * Time.deltaTime);
        pos.x = Mathf.Lerp(pos.x, Mathf.Cos(Time.time * frequency /2f) * amount * 1.6f, smoothing * Time.deltaTime);

        camTrans.localPosition += pos;

        return pos;
    }

    private void StopHeadBob()
    {
        if (camTrans.localPosition == startPos) return;
        camTrans.localPosition = Vector3.Lerp(camTrans.localPosition, startPos, 1 * Time.deltaTime);
    }
}
