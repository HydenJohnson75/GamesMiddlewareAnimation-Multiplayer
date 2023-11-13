using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot_Collider_Script : MonoBehaviour
{
    public bool footTouchedFloor;

    // Start is called before the first frame update
    void Start()
    {
        footTouchedFloor = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Floor")
        {
            footTouchedFloor = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Floor")
        {
            footTouchedFloor = false;
        }
    }
}
