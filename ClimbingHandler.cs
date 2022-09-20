using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;




public class ClimbingHandler : MonoBehaviour
{

    public GameObject raycastClimb;
    public GameObject raycastForward;

    PlayerMovement playermovement;

    public LayerMask ledgelayer;

    public PreviewCondition previewClimb;

    public bool ledgeClimb;

    public float ledgeDistance;


    private void Start()
    {
        previewClimb = PreviewCondition.Both;

    }


    public void ClimbingMechanics()
    {


        if (Physics.Raycast(raycastClimb.transform.position, -transform.up, out RaycastHit ledgeHit, Mathf.Infinity, ledgelayer, previewClimb, 1f, Color.blue, Color.gray) &&
    !ledgeClimb)
        {
            ledgeDistance = Vector3.Distance(transform.position, ledgeHit.transform.position);
            ledgeClimb = true;

        }


        if (ledgeClimb)

        {
            //transform.Translate(transform.localPosition + new Vector3(0, ledgeDistance, 0) * Time.deltaTime);
            Debug.Log($" Player is at {transform.position} Ledge is at {ledgeHit.point}!");

            // transform.position = Vector3.Lerp(transform.position, ledgeHit.point, Time.deltaTime);


            Vector3 ledgeTransportY = new Vector3(transform.position.x, ledgeHit.point.y, transform.position.z);
            playermovement.gravity = -1.0f;
            transform.position = Vector3.Lerp(transform.position, ledgeTransportY, .01f);

            if (Vector3.Distance(transform.position, ledgeTransportY) < .5)
            {
                ledgeClimb = false;
                // playermovement.gravity = -9.8f;

            }

        }






    }
}
