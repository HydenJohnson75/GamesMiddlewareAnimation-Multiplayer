using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Door : NetworkBehaviour, I_Interactable
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    doorStates currentState = doorStates.closed;
    enum doorStates { open, closed, opening, closing};

    public void Interact()
    {
        if(currentState == doorStates.open)
        {
            currentState = doorStates.closing;
        }
        else if (currentState == doorStates.closed)
        {
            currentState = doorStates.opening;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.GetPositionAndRotation(out originalPosition, out originalRotation);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (currentState == doorStates.opening)
        {
            if (transform.rotation != new Quaternion(0,90,0, 0))
            {
                transform.Rotate(0, 0.2f, 0);
            }
            else
            {
                currentState = doorStates.open;
            }
        }

        if (currentState == doorStates.closing)
        {
            if (transform.rotation != originalRotation)
            {
                transform.Rotate(0, -0.2f, 0);
            }
            else
            {
                currentState = doorStates.closed;
            }
        }
         
    }

}