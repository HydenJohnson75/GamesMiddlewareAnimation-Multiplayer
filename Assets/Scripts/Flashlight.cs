using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    Animator lightAnimator;
    [SerializeField] private GameObject handBone;

    // Start is called before the first frame update
    void Start()
    {
        lightAnimator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

    }


}
