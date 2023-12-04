using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Flashlight : NetworkBehaviour
{


    float rotationSpeed = 50f;
    float moveSpeed= 0.3f;
    private Vector3 startPosition;
    public bool isPlayerFlashlight;
    private enum movingStates {idle, movingUp, movingDown, reachedMaxHeight, reachedLowestHeight}
    private movingStates currentState;
    // Start is called before the first frame update
    void Start()
    {
        if(!isPlayerFlashlight)
        {
            startPosition = transform.position;
            currentState = movingStates.idle;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlayerFlashlight)
        {
            transform.Rotate(new Vector3(0, 0, 1) * rotationSpeed * Time.deltaTime);

            switch (currentState)
            {
                case movingStates.idle:
                    {
                        currentState = movingStates.movingUp;
                        break;
                    }
                case movingStates.movingUp:
                    {
                        MoveUp();
                        if (transform.position.y >= 1.25)
                        {
                            currentState = movingStates.reachedMaxHeight;
                        }
                        break;
                    }
                case movingStates.movingDown:
                    {
                        MoveDown();

                        if (transform.position.y <= startPosition.y)
                        {
                            currentState = movingStates.reachedLowestHeight;
                        }
                        break;
                    }
                case movingStates.reachedMaxHeight:
                    {
                        currentState = movingStates.movingDown;
                        break;
                    }
                case movingStates.reachedLowestHeight:
                    {
                        currentState = movingStates.movingUp;
                        break;
                    }

            }
        }


        
    }

    private void MoveUp()
    {
        transform.position += (new Vector3(0, 1, 0) * moveSpeed * Time.deltaTime);
    }

    private void MoveDown()
    {
        transform.position -= (new Vector3(0,1,0) * moveSpeed * Time.deltaTime);
    }
}
