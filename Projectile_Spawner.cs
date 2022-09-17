using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Projectile_Spawner : MonoBehaviour
{

    public Transform launcher;
    public LayerMask layerMask;


    // Update is called once per frame
    void Update()
    {
        //  RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        //        if (Physics.Raycast(launcher.transform.position, launcher.transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity, layerMask))
        // {
        //     Debug.DrawRay(launcher.transform.position, launcher.transform.TransformDirection(Vector3.up) * hit.distance, Color.yellow);
        // }
        // else
        // {
        //     Debug.DrawRay(launcher.transform.position, launcher.transform.TransformDirection(Vector3.up) * 1000, Color.white);


        // }
    }

}
