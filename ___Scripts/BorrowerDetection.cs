using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;



public class BorrowerDetection : MonoBehaviour
{

    public float detectionAngle;

    public GameObject eyesCast;
    public float seeingDistance = 10f;
    public LayerMask borrowersMask;

    public GameObject target;

    Animator animator;

    public bool targetDetected;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

        bool isDisguised = target.GetComponent<PlayerManager>().isDisguised;

        Vector3 heading = target.transform.position - transform.position;

        detectionAngle = Vector3.Dot(eyesCast.transform.forward.normalized, heading.normalized);

        //   Debug.Log(Vector3.Distance(transform.position, target.transform.position));

        if (detectionAngle > .7f && detectionAngle < 1.1f && !isDisguised)
        {

            if (Vector3.Distance(transform.position, target.transform.position) < seeingDistance)
            {
                targetDetected = true;
                Debug.Log("Version 2 Detection ");
                animator.SetBool("detected", true);
            }
            else
            {
                animator.SetBool("detected", false);
                targetDetected = false;

            }

        }
        else
        {
            animator.SetBool("detected", false);
            targetDetected = false;

        }
    }
}
