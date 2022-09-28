using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public Transform target;

    public float t;

    // Update is called once per frame
    void Update()
    {

        //SET POSITION
        //transform.position = new Vector3(1, 7, -5);
        // transform.position = target.position;



        //LERP

        // transform.position = Vector3.Lerp(transform.position, target.position, t);



    }






}

