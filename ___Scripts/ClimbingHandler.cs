using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;




public class ClimbingHandler : MonoBehaviour
{

    [Header("References")]
    Animator animator;
    PlayerManager playerManager;
    InputAction moveAction;
    PlayerControls playerControls;
    CharacterController controller;




    [Header("Vectors")]


    public Vector3 ledgePosition;
    public Vector3 ledgeTransportPlayer;



    [Header("HangMovement")]

    public Vector2 hangMoveInput;
    public Vector3 hangMovement;

    public Vector3 climbableOrientation;


    [Header("Bools")]

    public bool ledgeClimb;

    public bool hangingMovementEnabled;

    [Header("Floats")]

    public float lerpRatio = .1f;
    public float playerHeight = 2.5f;
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

    public Vector3 lastValidHangPoint;


    private void OnEnable()
    {
        moveAction = playerControls.Player.Move;

        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
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
        //playerForward = transform.forward;

        if (Physics.Raycast(raycastClimb.transform.position, -transform.up, out RaycastHit ledgeHit, Mathf.Infinity, ledgelayer, previewClimb, 1f, Color.green, Color.red) && !ledgeClimb)
        {

            ledgePosition = ledgeHit.point;


            //RAYCAST to find the normal of the object we are about to climb
            if (!hangingMovementEnabled && Physics.Raycast(raycastForward.transform.position, transform.forward, out RaycastHit objectHit, raycastLength, climbLayer, previewClimb, 1f, Color.blue, Color.red))
            {

                climbableOrientation = objectHit.normal;  //direction the object faces 
                transform.forward = -climbableOrientation;

                if (objectHit.rigidbody != null)
                {
                    //find the object's rigid body
                    objectRB = objectHit.rigidbody;
                    objectRB.GetComponent<Rigidbody>().isKinematic = true;
                    //get the difference in height between the player and the downward collision
                }

                objectGO = objectHit.transform.gameObject;

                //position of ledge with player adjsutments for hieghta nd width
                ledgeTransportPlayer = new Vector3(ledgePosition.x, ledgePosition.y - playerHeight, ledgePosition.z - playerWidth);

                playerManager.ChangeState(PlayerManager.PlayerState.Climbing);  //remove player controls
                animator.SetBool("isHanging", true);

                ledgeClimb = true;


            }

        }

        if (ledgeClimb)
        {
            Debug.Log("Ledgeclimb started");

            //animation change
            transform.position = Vector3.Lerp(transform.position, ledgeTransportPlayer, lerpRatio);
            Debug.Log($"Wow, ledge position is {ledgeTransportPlayer.y} and player is at {transform.position.y} = {ledgeTransportPlayer.y - transform.position.y}");

            if (((ledgeTransportPlayer.y) - transform.position.y) < .01f)

            {
                ledgeClimb = false;
                hangingMovementEnabled = true;

            }
        }


        if (hangingMovementEnabled && !ledgeClimb && Physics.Raycast(raycastClimb.transform.position, -transform.up, out RaycastHit ledgeGroundCheck, Mathf.Infinity, ledgelayer, previewClimb, 1f, Color.green, Color.red))
        {

            Debug.Log("Hanging controls Enabled");

            hangMoveInput = moveAction.ReadValue<Vector2>();

            hangMovement = new Vector3(hangMoveInput.x, 0, 0);

            lastValidHangPoint = transform.position;


            // Vector3 hangMoveMin = objectGO.GetComponent<Renderer>().bounds.min;
            // Vector3 hangMoveMax = objectGO.GetComponent<Renderer>().bounds.max;

            // if (transform.position.x > hangMoveMin.x && transform.position.x < hangMoveMax.x)
            controller.Move(hangMovement.x * transform.right * Time.deltaTime);
        }
        else if (hangingMovementEnabled && !ledgeClimb)
        {
            transform.position = lastValidHangPoint;
        }





    }




}

