using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;
using TMPro;
using DG.Tweening;



public class ClimbingHandler : MonoBehaviour
{

    [Header("References")]
    Animator animator;
    PlayerManager playerManager;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction crouchAction;
    PlayerControls playerControls;
    CharacterController controller;

    [Header("Vectors")]

    public Vector3 ledgeLocation;


    [Header("HangMovement")]
    public Vector3 climbableOrientation;


    [Header("Bools")]
    public bool canClimb = false;
    public bool hangingMovementEnabled;


    [Header("Floats")]
    public float forwardRayLength;


    [Header("External Links")]

    public GameObject raycastClimb;
    public GameObject raycastForward;

    public LayerMask ledgelayer;
    public PreviewCondition previewClimb;
    public LayerMask climbLayer;


    public float timeElapsed;
    public float distanceFromLedge;


    //-----------//  //-----------//
    //-----------//  //-----------//


    private void OnEnable()
    {
        moveAction = playerControls.Player.Move;

        moveAction.Enable();

        jumpAction = playerControls.Player.Jump;
        jumpAction.Enable();

        crouchAction = playerControls.Player.Crouch;
        crouchAction.Enable();
    }


    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        crouchAction.Disable();
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
        controller = GetComponent<CharacterController>();
    }


    private void Start()
    {
        previewClimb = PreviewCondition.Both;

        playerManager = GetComponent<PlayerManager>();

        animator = GetComponent<Animator>();

    }

    //-----------//  //-----------//
    //-----------//  //-----------//

    public void ClimbingMechanics()
    {

        //This raycast detects climable objects, grounds the player to a ledge while hanging and moving, and many other things.
        if (Physics.Raycast(raycastClimb.transform.position, -transform.up, out RaycastHit ledgeHit, 3f, ledgelayer, previewClimb, 1f, Color.green, Color.red))
        {
            ledgeLocation = ledgeHit.point;

            if (Physics.Raycast(raycastForward.transform.position, transform.forward, out RaycastHit forwardHit, 2f, ledgelayer, previewClimb, 1f, Color.green, Color.red))

                climbableOrientation = -forwardHit.normal;


            Debug.Log("Can climb");

            canClimb = true;

            if (jumpAction.IsPressed())
            {
                distanceFromLedge = forwardHit.distance;

                StartCoroutine("LedgeClimb");

            }
        }
        else
        { canClimb = false; }

    }


    IEnumerator LedgeClimb()
    {
        {
            playerManager.ChangeState(PlayerManager.PlayerState.Climbing);
            animator.SetBool("isHanging", true);

            transform.forward = climbableOrientation;

            yield return new WaitForSeconds(.13f);

            //transform.localPosition += new Vector3(0, 0, distanceFromLedge);

            Vector3 ledgePositionAdjusted = new Vector3(transform.position.x, ledgeLocation.y - controller.height * .85f, transform.position.z);

            while (ledgePositionAdjusted.y - transform.position.y > .01)
            {
                transform.position = Vector3.Lerp(transform.position, ledgePositionAdjusted, Time.deltaTime);
                //transform.position = Vector3.MoveTowards(transform.position, ledgeLocation, Time.deltaTime);
                //transform.position += new Vector3(0, ledgeLocation.y, 0);
                yield return null;
                Debug.Log("Do I stop?");
            }
            playerManager.ChangeState(PlayerManager.PlayerState.Hanging);


        }
    }




}