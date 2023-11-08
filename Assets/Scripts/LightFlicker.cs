using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light lightObj;

    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;


    public float timer;



    // Start is called before the first frame update
    void Start()
    {
        timer = Random.Range(minTime, maxTime);

    }

    // Update is called once per frame
    void Update()
    {
        FlickerLight();
    }

    private void FlickerLight()
    {
        if(timer > 0)
        {
            timer-= Time.deltaTime;
        }

        if(timer <= 0)
        {
            lightObj.enabled = !lightObj.enabled;
            timer = Random.RandomRange(minTime, maxTime);
        }

    }
}
