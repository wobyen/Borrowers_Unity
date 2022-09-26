using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;


public class Movement : MonoBehaviour
{
    public bool controlsEnabled;
    public bool lerpTest;
    public bool raycastTest;

    public float accelForce;
    public float accelRate;
    public float decelRate = 4;

    public Vector3 storedMoveDir;

    public Vector3 localMoveDir;
    public float maxSpeed = 4;

    [Header("CACHED")]
    [SerializeField] GameObject targetObject;
    [SerializeField] GameObject raycastGO;
    CharacterController controller;
    PlayerControls playerControls;

    JumpHandler jumphandler;
    Animator animator;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;
    InputAction crouchAction;

    public AnimationCurve accelCurve;

    public float timePassed;


    private void Awake()
    {
        playerControls = new PlayerControls();

        controller = GetComponent<CharacterController>();

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


    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        jumphandler = GetComponent<JumpHandler>();
        animator = GetComponent<Animator>();
    }

    public void defaultMovement()

    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        animator.SetFloat("velocityX", moveDirection.x * accelRate);
        animator.SetFloat("velocityZ", moveDirection.z * accelRate);


        //makes input match player local space
        localMoveDir = transform.TransformDirection(moveDirection);

        playerMomentum();

        playerRotation();

        if (jumphandler.playerGrounded)
        {
            controller.Move(Vector3.Lerp(Vector3.zero, localMoveDir, accelRate));  //otherwise just move the player.
        }

        if (raycastTest)
        {
            if (Physics.Raycast(raycastGO.transform.position, transform.forward, out RaycastHit hitInfo, PreviewCondition.Both, 2f, Color.green, Color.red))
            {
                Debug.Log(hitInfo.normal);

                transform.forward = -hitInfo.normal;
            }
        }

    }


    void playerMomentum()
    { //if player is moving, accerlation force increases over time * amount of magnitude of input.  Cannot exceed max speed
        if (localMoveDir != Vector3.zero && jumphandler.playerGrounded)
        {
            accelForce += Time.deltaTime * localMoveDir.sqrMagnitude;
            if (accelForce > maxSpeed)
                accelForce = maxSpeed;
        }

        else if (localMoveDir != Vector3.zero && !jumphandler.playerGrounded)  //if player is in the air jumping they do not gain speed
        {
            if (accelForce > maxSpeed)
                accelForce = maxSpeed;
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


    void playerRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);   // find the camera's Y rotation
        float rotationSpeed = 7;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    }


}