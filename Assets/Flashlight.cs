using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Flashlight : NetworkBehaviour
{
    Animator lightAnimator;

    // Start is called before the first frame update
    void Start()
    {
        lightAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pickUp()
    {
        lightAnimator.enabled = false;
    }
}
