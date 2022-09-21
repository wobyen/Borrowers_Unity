using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;




public class ClimbingHandler : MonoBehaviour
{

    public Vector2 hangMoveInput;
    public bool hangingMovementEnabled;

    PlayerMovement playermovement;
    PlayerManager playerManager;

    Animator animator;

    public GameObject raycastClimb;
    public GameObject raycastForward;

    public Vector3 playerForward;

    public LayerMask ledgelayer;

    public PreviewCondition previewClimb;

    public bool ledgeClimb;

    public float ledgeDistance;

    public float playerHeight = 2.5f;
    public float playerWidth = .5f;

    public Vector3 climbableOrientation;
    public Vector3 ledgePosition;

    public LayerMask climbLayer;

    Rigidbody objectRBRB;

    InputAction moveAction;

    PlayerControls playerControls;

    CharacterController controller;
    public Vector3 hangMovement;

    public float raycastLength;

    public Rigidbody objectRB;
    public Vector3 ledgeTransportPlayer;

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

            //RAYCAST to find the normal of the object we are about to climb
            if (Physics.Raycast(raycastForward.transform.position, transform.forward, out RaycastHit objectHit, raycastLength, climbLayer, previewClimb, 1f, Color.blue, Color.red))
            {

                climbableOrientation = objectHit.normal;  //direction the object faces 

                if (objectHit.rigidbody != null)
                {
                    //find the object's rigid body
                    objectRB = objectHit.rigidbody;
                    objectRB.GetComponent<Rigidbody>().isKinematic = true;
                    //get the difference in height between the player and the downward collision
                }

                ledgePosition = ledgeHit.point;
                //start next stage

                transform.forward = -climbableOrientation;

                //position of ledge with player adjsutments for hieghta nd width
                ledgeTransportPlayer = new Vector3(ledgePosition.x, ledgePosition.y - playerHeight, ledgePosition.z - playerWidth);
                playerManager.ChangeState(PlayerManager.PlayerState.Climbing);  //remove player controls
                animator.SetBool("isHanging", true);



                ledgeClimb = true;
            }




            if (ledgeClimb)
            {
                Debug.Log("Ledgeclimb started");

                //animation change

                transform.position = Vector3.Lerp(transform.position, ledgeTransportPlayer, .1f);
                Debug.Log($"Wow, ledge position is {ledgeTransportPlayer.y} and player is at {transform.position.y}.");

                if ((ledgeTransportPlayer.y - transform.position.y < .5))

                {
                    hangingMovementEnabled = true;
                    ledgeClimb = false;
                }
            }


            if (hangingMovementEnabled && Physics.Raycast(raycastClimb.transform.position, -transform.up, out RaycastHit ledgeGroundCheck, Mathf.Infinity, ledgelayer, previewClimb, 1f, Color.green, Color.red))
            {

                Debug.Log("Hanging controls Enabled");

                hangMoveInput = moveAction.ReadValue<Vector2>();

                hangMovement = new Vector3(hangMoveInput.x, 0, 0);

                controller.Move(hangMovement.x * transform.right * Time.deltaTime);

            }



        }
    }

}
