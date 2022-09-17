using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PushObjects : MonoBehaviour
{

    [SerializeField] private float forceMag;

    InputAction useAction;

    PlayerControls playerControls;

    Vector3 forceDirection;

    Rigidbody pushableRB;

    public bool canPush;

    private void OnEnable()
    {
        useAction = playerControls.Player.Use;
        useAction.Enable();

        useAction.canceled += context => canPush = false;
    }

    private void OnDisable()
    {
        useAction.Disable();

    }
    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        pushableRB = hit.collider.attachedRigidbody;

        if (pushableRB != null && useAction.IsPressed())
        {
            canPush = true;

            Debug.Log("Trying to push");
            forceDirection = hit.gameObject.transform.position - transform.position;

        }

    }


    public void PushingObject()

    {
        if (canPush)
        {



            forceDirection.y = 0;
            forceDirection.Normalize();

            pushableRB.AddForceAtPosition(forceMag * forceDirection, transform.position, ForceMode.Impulse);

        }


    }



}
