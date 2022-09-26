using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitch : MonoBehaviour
{


    public CinemachineClearShot newCamera;



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            newCamera.Priority += 100;

        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            newCamera.Priority -= 100;

        }
    }

}