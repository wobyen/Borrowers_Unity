using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{

    int jumps = 0;

    private void Awake()
    {


    }



    public void JumpCounter()
    {

        jumps++;

        Debug.Log(jumps);



    }



}