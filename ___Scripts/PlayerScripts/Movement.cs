using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;


public class Movement : InputManager
{
    public bool controlsEnabled;
    public bool lerpTest;
    public bool raycastTest;

    public float accelForce;
    public float accelRate;
    public float decelRate = 4;

    [SerializeField] float airDecelRate = 1;

    public Vector3 storedMoveDir;

    public Vector3 localMoveDir;
    public float maxSpeed = 4;

    [Header("CACHED")]
    [SerializeField] GameObject targetObject;
    [SerializeField] GameObject raycastGO;
    CharacterController controller;
    public PlayerControls playerControls;

    GroundCheck groundCheck;
    Animator animator;

    // InputAction moveAction;
    // InputAction sprintAction;

    Vector2 moveInput;

    public AnimationCurve accelCurve;

    public float timePassed;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        groundCheck = GetComponent<GroundCheck>();
        animator = GetComponent<Animator>();

    }


    public void defaultMovement()

    {
        moveInput = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        animator.SetFloat("velocityX", moveDirection.x * accelRate);
        animator.SetFloat("velocityZ", moveDirection.z * accelRate);


        //makes input match player local space
        localMoveDir = transform.TransformDirection(moveDirection);

        playerMomentum();

        if (groundCheck.isGrounded())
        {
            controller.Move(Vector3.Lerp(Vector3.zero, localMoveDir, accelRate));  //otherwise just move the player.
        }

        // if (raycastTest)
        // {
        //     if (Physics.Raycast(raycastGO.transform.position, transform.forward, out RaycastHit hitInfo, PreviewCondition.Both, 2f, Color.green, Color.red))
        //     {
        //         Debug.Log(hitInfo.normal);

        //         transform.forward = -hitInfo.normal;
        //     }
        // }

    }


    void playerMomentum()
    { //if player is moving, accerlation force increases over time * amount of magnitude of input.  
      //Cannot exceed max speed

        if (localMoveDir != Vector3.zero && groundCheck.isGrounded())
        {
            accelForce += Time.deltaTime * localMoveDir.sqrMagnitude;
            if (accelForce > maxSpeed)
                accelForce = maxSpeed;
        }

        else if (localMoveDir != Vector3.zero && !groundCheck.isGrounded())
        {
            accelForce -= Time.deltaTime * airDecelRate;

            if (accelForce < 0)
            {
                accelForce = 0;
                storedMoveDir = Vector3.zero;
            }
        }

        else // otherwise, decellerate character at rate.  do not decellerate past 0.
        {
            accelForce -= Time.deltaTime * decelRate;

            if (accelForce < 0)
            {
                accelForce = 0;
                storedMoveDir = Vector3.zero;
            }
        }

        accelRate = accelCurve.Evaluate(accelForce);  //use curve to smooth acceration

        if (localMoveDir != Vector3.zero)
            storedMoveDir = localMoveDir;  //if player is moving, store last valid direction

        else
        {
            controller.Move(storedMoveDir * Time.deltaTime);  //if they stop moving, apply last valid direction while decellerating
        }
    }


    public void playerRotationCamera()
    {
        Quaternion targetRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);   // find the camera's Y rotation
        float rotationSpeed = 7;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    }



}