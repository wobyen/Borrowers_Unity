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

    public float climbUpHeightAdjust;
    public float hangFromLedgeHeightAdjust;


    [Header("References")]
    Animator animator;
    PlayerManager playerManager;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction crouchAction;
    PlayerControls playerControls;
    CharacterController controller;




    [Header("Vectors")]
    public Vector3 landingZone;

    public float landingzoneHeightAdjust;

    public Vector3 lastValidHangPoint;

    public Vector3 ledgePosition;
    public Vector3 ledgeTransportPlayer;



    [Header("HangMovement")]

    public Vector2 hangMoveInput;
    public Vector3 hangMovement;

    public Vector3 climbableOrientation;


    [Header("Bools")]
    public bool canClimb = false;
    public bool ledgeClimb;

    public bool hangingMovementEnabled;

    [Header("Floats")]

    public float lerpRatio = .1f;
    public float playerHeight = 1.8f;
    public float playerWidth = .5f;

    public float raycastLength;



    [Header("External Links")]


    public GameObject raycastClimb;
    public GameObject raycastForward;
    public LayerMask ledgelayer;
    public PreviewCondition previewClimb;

    public LayerMask climbLayer;

    public Rigidbody objectRB;

    GameObject objectGO;
    public TextMeshProUGUI jumpText;

    public float distanceFromObject;


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

    public void ClimbingMechanics()
    {

        //This raycast detects climable objects, grounds the player to a ledge while hanging and moving, and many other things.
        if (Physics.Raycast(raycastClimb.transform.position, -transform.up, out RaycastHit ledgeHit, 3f, ledgelayer, previewClimb, 1f, Color.green, Color.red) && !ledgeClimb)
        {

            ledgePosition = ledgeHit.point;

            if (ledgeHit.point.y > transform.position.y)  //if the collision is higher, than the player can climb up
                canClimb = true;


            animator.SetBool("isDropping", false);
            //RAYCAST to find the normal of the object we are about to climb


            if (jumpAction.triggered && !hangingMovementEnabled && Physics.Raycast(raycastForward.transform.position, transform.forward, out RaycastHit objectHit, raycastLength, climbLayer, previewClimb, 1f, Color.blue, Color.red))
            {

                climbableOrientation = objectHit.normal;  //direction the object faces 
                transform.forward = -climbableOrientation;

                if (objectHit.rigidbody != null)
                {
                    //find the object's rigid body
                    objectRB = objectHit.rigidbody;
                    objectRB.GetComponent<Rigidbody>().isKinematic = true;
                }

                objectGO = objectHit.transform.gameObject;  //Get the GameObject in case it's needed
                distanceFromObject = objectHit.distance;  // find the distance between player and game object


                //the position the player will be in when they are hanging
                ledgeTransportPlayer = new Vector3(transform.position.x, ledgePosition.y - hangFromLedgeHeightAdjust, transform.position.z);


                playerManager.ChangeState(PlayerManager.PlayerState.Climbing);  //remove player controls
                animator.SetBool("isHanging", true);

                ledgeClimb = true;

            }

        }
        else
        {
            canClimb = false;

        }

        if (ledgeClimb)
        {
            Debug.Log("Ledgeclimb started");


            //move player towards object
            transform.position += transform.forward * distanceFromObject;

            //move player into HANG POSITON
            transform.DOMove(ledgeTransportPlayer, 1f);


            if (((ledgeTransportPlayer.y) - transform.position.y) < .01f)  //wait until done, then move to next state

            {
                ledgeClimb = false;
                hangingMovementEnabled = true;
            }
        }


        if (hangingMovementEnabled && !ledgeClimb && Physics.Raycast(raycastClimb.transform.position, -transform.up, out RaycastHit ledgeGroundCheck, Mathf.Infinity, ledgelayer, previewClimb, 1f, Color.green, Color.red))
        {

            hangMoveInput = moveAction.ReadValue<Vector2>();
            hangMovement = new Vector3(hangMoveInput.x, 0, 0);  //move inputs for moving side to side

            //giving player control to move side to side
            controller.Move(hangMovement.x * transform.right * Time.deltaTime);

            lastValidHangPoint = transform.position;  //last valid point saved in case character moves off edge

            //the place the character will land when climbing up onto object
            landingZone = new Vector3(ledgeGroundCheck.point.x, ledgeGroundCheck.point.y - climbUpHeightAdjust, ledgeGroundCheck.point.z);


        }
        else if (hangingMovementEnabled && !ledgeClimb)  //if player tries to move off edge while hanging, push them back to last valid location
        {
            transform.position = lastValidHangPoint;
        }

        if (hangingMovementEnabled && crouchAction.triggered) //if player presses crouch, they drop down
        {
            hangingMovementEnabled = false;
            playerManager.ChangeState(PlayerManager.PlayerState.BasicMovement);
            animator.SetBool("isDropping", true);

        }
        if (hangingMovementEnabled && jumpAction.triggered)  //if the player jumps, they climb up onto ledge.
        {
            StartCoroutine(ClimbUpAnimation(landingZone));
        }

    }


    IEnumerator ClimbUpAnimation(Vector3 landingZone)  //player climbs up onto ledge
    {

        hangingMovementEnabled = false;

        animator.SetBool("isClimbing", true);

        transform.DOMove(landingZone, 1.3f).SetEase(Ease.InOutQuad);

        yield return null;

        playerManager.ChangeState(PlayerManager.PlayerState.BasicMovement);
        animator.SetBool("isClimbing", false);
        animator.SetBool("isHanging", false);

        StopAllCoroutines();
    }

}

