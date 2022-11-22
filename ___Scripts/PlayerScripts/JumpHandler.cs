using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;

public class JumpHandler : InputManager
{

    public bool playerGrounded;
    public bool jumping;

    public bool jumpButtonPressed = false;

    public float jumpInputTimer;
    public float jumpInputTimerMax = 1f;

    public float storedMomentum;
    public float jumpTime;

    CharacterController controller;
    Movement playerMovement;
    GravityHandler gravityHandler;

    ClimbableDetection climbableDetection;

    public float timePassed;
    public float jumpForce;

    GroundCheck groundCheck;

    [SerializeField] AnimationCurve jumpCurve;


    private void Awake()
    {
        //  playerControls = new PlayerControls();

        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<Movement>();
        gravityHandler = GetComponent<GravityHandler>();
        controller = GetComponent<CharacterController>();
        climbableDetection = GetComponent<ClimbableDetection>();
        groundCheck = GetComponent<GroundCheck>();
    }

    public void JumpMechanics()

    {

        if (jumpAction.IsPressed() && !climbableDetection.canClimb)
        {
            jumpInputTimer = 0;

            jumpButtonPressed = true;
        }

        if (jumpButtonPressed && jumpInputTimer < jumpInputTimerMax)

        {
            jumpInputTimer += Time.deltaTime;
        }

        else
        {
            jumpButtonPressed = false;
            jumpInputTimer = 0;
        }

        //IS PLAYER GROUNDED
        if (groundCheck.isGrounded())
        {
            if (jumpButtonPressed && !climbableDetection.canClimb)
            {
                Debug.Log("jump");
                jumping = true;
            }
        }
        else  //otherwise, they ain't groudned so apply gravity
        {
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
