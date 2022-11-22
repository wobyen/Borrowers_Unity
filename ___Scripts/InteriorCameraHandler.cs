using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;



public class InteriorCameraHandler : MonoBehaviour
{
    public CinemachineFreeLook newCam;

    PlayerManager playerManager;


    private void Awake()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            newCam.Priority = 100;

            //other.GetComponent<PlayerManager>().ChangeState(PlayerManager.PlayerState.FixedCamMovement);


        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            newCam.Priority = 0;
            // other.GetComponent<PlayerManager>().ChangeState(PlayerManager.PlayerState.BasicMovement);

        }

    }



}
