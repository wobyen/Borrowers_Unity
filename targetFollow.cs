using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetFollow : MonoBehaviour
{

    //Parents an empty object within the playerRig that the palyer will aim at.

    public Transform target;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        transform.position = target.transform.position;
    }
}
