using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class ledgeVolume : MonoBehaviour
{

    LedgeHandler LedgeHandler;

    private void Start()
    {
        LedgeHandler = GetComponent<LedgeHandler>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LedgeHandler.hangingMovementEnabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        LedgeHandler.hangingMovementEnabled = false;


    }


    private void FixedUpdate()
    {




    }




}
