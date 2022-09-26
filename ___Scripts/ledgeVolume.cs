using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class ledgeVolume : MonoBehaviour
{

    ClimbingHandler climbingHandler;

    private void Start()
    {
        climbingHandler = GetComponent<ClimbingHandler>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            climbingHandler.hangingMovementEnabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        climbingHandler.hangingMovementEnabled = false;


    }


    private void FixedUpdate()
    {




    }




}
