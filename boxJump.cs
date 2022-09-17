using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class boxJump : MonoBehaviour
{

    [SerializeField] AnimationCurve boxJumpCurve;
    public Vector3 boxPosition;



    private void Awake()
    {

        boxPosition = transform.position;


    }
    // Update is called once per frame
    void Update()
    {

        transform.position = new Vector3(transform.position.x, boxJumpCurve.Evaluate(Time.time), transform.position.z);

        //boxPosition.x += boxPosition.x + 1;


    }
}
