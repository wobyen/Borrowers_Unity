using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;
using TMPro;
using DG.Tweening;



public class LedgeHandler : MonoBehaviour
{

    //-----// CACHED REFS //-----//

    Animator animator;
    PlayerManager playerManager;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction crouchAction;
    PlayerControls playerControls;
    CharacterController controller;
    JumpHandler jumpHandler;



    //-----// VECTORS  //-----//

    [Header("Vectors")]

    [Header("HangMovement")]
    public Vector3 climbableOrientation;
    public Vector3 ledgeLocation;


    //-----// BOOLS  //-----//


    [Header("Bools")]
    public bool canClimb = false;
    public bool hangingMovementEnabled;



    //-----// FLOATS   //-----//

    [Header("Floats")]
    public float distanceFromLedge;



    //-----// LINKS   //-----//


    [Header("External Links")]

    public GameObject raycastClimb;
    public GameObject raycastForward;
    public LayerMask ledgelayer;
    public PreviewCondition previewClimb;

    public Collider ledgeCollider;


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

        jumpHandler = GetComponent<JumpHandler>();
    }


    private void Start()
    {
        previewClimb = PreviewCondition.Both;

        playerManager = GetComponent<PlayerManager>();

        animator = GetComponent<Animator>();

    }

    //-----------//  //-----------//
    //-----------//  //-----------//

    public void LedgeMechanics() //DETECTS IF PLAYER CAN CLIMB 
    {

        //This raycast detects climable objects, grounds the player to a ledge while hanging and moving, and many other things.
        if (Physics.Raycast(raycastClimb.transform.position, -transform.up, out RaycastHit ledgeHit, 3f, ledgelayer, previewClimb, 1f, Color.green, Color.red))
        {
            ledgeLocation = ledgeHit.point;

            ledgeCollider = ledgeHit.collider;

            canClimb = true;  //disables jump when player can climb so that they climb instead.


            if (Physics.Raycast(raycastForward.transform.position, transform.forward, out RaycastHit forwardHit, 2f, ledgelayer, previewClimb, 1f, Color.green, Color.red))

                climbableOrientation = -forwardHit.normal;



            if (jumpAction.IsPressed() && !jumpHandler.jumping)
            {
                distanceFromLedge = forwardHit.distance;

                Vector3 ledgePositionAdjusted = new Vector3(transform.position.x, ledgeLocation.y - controller.height * .85f, transform.position.z);

                StartCoroutine("LedgeClimb", ledgePositionAdjusted);

                playerManager.ChangeState(PlayerManager.PlayerState.Hanging);

            }
        }
        else
        { canClimb = false; }

    }





    public IEnumerator LedgeClimb(Vector3 ledgePositionAdjusted)
    {
        playerManager.ChangeState(PlayerManager.PlayerState.Climbing);

        animator.SetBool("isHanging", true);

        transform.forward = climbableOrientation;

        yield return new WaitForSeconds(.13f);  //wait for animation

        transform.DOMove(ledgePositionAdjusted, .5f);

        // while (ledgePositionAdjusted.y - transform.position.y > .5f)
        // {
        //     transform.position = Vector3.Lerp(transform.position, ledgePositionAdjusted, Time.deltaTime);

        //     yield return null;
        // }


    }




}