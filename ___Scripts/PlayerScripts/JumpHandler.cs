using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;
public class JumpHandler : MonoBehaviour
{

    public bool playerGrounded;
    public bool jumping;

    public float storedMomentum;
    public float jumpTime;
    [SerializeField] GameObject raycastGO;
    CharacterController controller;
    PlayerControls playerControls;
    Movement playerMovement;
    GravityHandler gravityHandler;
    ClimbingHandler climbingHandler;

    Animator animator;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;
    InputAction crouchAction;

    public float timePassed;

    public float jumpForce;

    [SerializeField] AnimationCurve jumpCurve;


    private void Awake()
    {
        playerControls = new PlayerControls();

        controller = GetComponent<CharacterController>();

        playerMovement = GetComponent<Movement>();

        gravityHandler = GetComponent<GravityHandler>();
        climbingHandler = GetComponent<ClimbingHandler>();
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();

        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        moveAction = playerControls.Player.Move;
        moveAction.Enable();

        jumpAction = playerControls.Player.Jump;
        jumpAction.Enable();

        sprintAction = playerControls.Player.Sprint;
        sprintAction.Enable();

        crouchAction = playerControls.Player.Crouch;
        crouchAction.Enable();
    }


    private void OnDisable()
    {
        moveAction.Disable();

        jumpAction.Disable();

        sprintAction.Disable();

        crouchAction.Disable();
    }


    public void JumpMechanics()

    {
        animator.SetBool("isGrounded", playerGrounded);

        //IS PLAYER GROUNDED
        if (Physics.Raycast(raycastGO.transform.position, Vector3.down, out RaycastHit hitInfo, .5f, PreviewCondition.Both, 1f, Color.green, Color.red))
        {
            playerGrounded = true;  //if the player is grounded and they press jump, they will jump.

            if (jumpAction.IsPressed() && !climbingHandler.canClimb)
            {
                jumping = true;
            }
        }
        else  //otherwise, they ain't groudned so apply gravity
        {
            playerGrounded = false;

            Vector3 gravityVector = new Vector3(playerMovement.storedMoveDir.x, gravityHandler.gravity, playerMovement.storedMoveDir.z);

            controller.Move(gravityVector * Time.deltaTime);
        }


        if (jumping)  //if jumping is true, gravity is 0 and they jump
        {
            storedMomentum = playerMovement.accelRate;
            StartCoroutine(PlayerJump());

        }




    }


    IEnumerator PlayerJump()
    {
        timePassed += Time.deltaTime;

        jumpForce = jumpCurve.Evaluate(timePassed);  //evaluate the animation curve based on time passed through jump



        controller.Move(new Vector3(playerMovement.storedMoveDir.x * storedMomentum, jumpForce * Time.deltaTime, playerMovement.storedMoveDir.z * storedMomentum));

        if (timePassed > jumpTime)
        {
            timePassed = 0;
            jumping = false;  //when we are out of time, stop.

        }



        yield return null;
    }
}
