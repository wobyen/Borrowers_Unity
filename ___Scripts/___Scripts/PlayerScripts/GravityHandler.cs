using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityHandler : MonoBehaviour
{


    public float gravity { get; private set; }


    private void Start()
    {
        gravity = -9;
    }

    public float GravityControls(float newGrav)
    {

        gravity = newGrav;
        return gravity;
    }






}
