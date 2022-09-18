using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;



public class PlayerMovement : MonoBehaviour
{
    //---------------- // CACHE //------------//

    CharacterController controller;
    PlayerControls playerControls;
    Animator animator;
    [SerializeField] GameObject raycaster;
    [SerializeField] PreviewCondition previewConditionGround;


    //---------------- // INPUT //------------//
    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;
    InputAction crouchAction;
    public float currentTime;
    public float jumpTime;

    [Header("BOOLS")]
    //---------------- // BOOLS //------------//
    public bool groundedPlayer;
    public bool canJump = false;


    //---------------- // MOVEMENT //------------//
    [Header("MOVE")]

    public Vector3 groundingRay;

    public LayerMask groundLayer;
    public Vector2 moveInput;
    public Vector3 moveDirection;

    public float playerSpeed = 0;

    public float walkMinSpeed = 0;

    public float walkMaxSpeed = 8;

    float rotationSpeed = 7;

    public float storedVelocity = 0;

    public Vector3 lastDirection;

    float slowRate = 50;

    float playerAcceleration = 0;

    //---------------- // JUMP //------------//
    [Header("JUMP")]

    [SerializeField] AnimationCurve jumpAnimCurve;
    public float gravity = -9.8f;


    //---------------- //  //------------//
    //---------------- //  //------------//
    //---------------- //  //------------//     



    private void Awake()
    {
        playerControls = new PlayerControls();

        controller = GetComponent<CharacterController>();

        playerSpeed = 5;

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
        animator = GetComponent<Animator>();
        playerControls = new PlayerControls();

        playerAcceleration = Mathf.Clamp(playerAcceleration, 0f, 8f);

    }

    //----------------//  //------------//
    //----------------//  //------------//
    //----------------//  //------------//


    // Update is called once per frame

    public void defaultMovement()
    {
        // Debug.Log($"World Rotation is {transform.rotation}");
        // Debug.Log($"Local Rotation is {transform.localRotation}");

        // Debug.Log($"Local Euler Angles is {transform.localEulerAngles}");
        // Debug.Log($" Euler Angles is {transform.eulerAngles}");

        groundingCheck();   //check if the player is grounded -- bool

        if (groundedPlayer)  //if player is groudned
        {
            animator.SetBool("isGrounded", true);  //animations when player is on the ground
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);

            moveInput = moveAction.ReadValue<Vector2>();  //input from player move 

            moveDirection = new Vector3(moveInput.x, 0, moveInput.y);  // converting player move to a Vector3

            playerSpeed = Mathf.Lerp(walkMinSpeed, walkMaxSpeed, moveInput.SqrMagnitude());  //lerp from min speed to max, the input magnitude = T;

            animator.SetFloat("velocityX", moveDirection.x * playerSpeed);  //animations based on player speed
            animator.SetFloat("velocityZ", moveDirection.z * playerSpeed);


            if (moveInput == Vector2.zero)   //if player is not moving
            {
                lastDirection.y = 0;
                controller.Move(lastDirection * storedVelocity * Time.deltaTime);

                playerSpeed = storedVelocity;

                storedVelocity -= slowRate * Time.deltaTime;

                animator.Play("Walking");
                animator.speed = playerSpeed;

                if (storedVelocity < 0)
                {
                    storedVelocity = 0;
                }

            }
            else
            {
                animator.speed = 1;
            }

            //adjusts moveDirection normals to reorient the player for the camera correctly

            moveDirection = moveDirection.x * Camera.main.transform.right.normalized + moveDirection.z * Camera.main.transform.forward.normalized;

            if (moveDirection != Vector3.zero)   //storing momentum data
            {
                lastDirection = moveDirection;
                storedVelocity = playerSpeed;
            }

            jumpingMechanics();  // while grounded, if CanJump = true, how does the jump work?

        }
        else //not grounded //fallling
        {
            animator.SetBool("isFalling", true);
            moveDirection.y += gravity * Time.deltaTime;  //if player is not grounded, they are falling.
        }


        jumpHandler();  //when true, facilitates jump logic with timers


        controller.Move(moveDirection * playerSpeed * Time.deltaTime);  // the input moves the player


        //rotate the player with camera
        Quaternion targetRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);   // find the camera's Y rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // animate the rotation with LERP
                                                                                                                  //controller.Move(moveDirection * playerSpeed * Time.deltaTime);
    }


    //------
    //------   METHODS
    //------


    void jumpHandler()
    {
        if (canJump)
        {
            jumpTime += Time.deltaTime;  //jump timer start
            moveDirection.y = jumpAnimCurve.Evaluate(jumpTime);  //jump height based on curve

            if (jumpTime > 0.5F)  //time for jump to complete
            {
                canJump = false;
            }

        }
    }


    void jumpingMechanics()  //how the jump works
    {
        if (!canJump)
            moveDirection.y = 0;  //player y axis is constrained

        if (jumpAction.triggered && !canJump)  //if jump is pressed and jump is over
        {
            animator.SetBool("isJumping", true);

            jumpTime = 0;    //reset jump timer

            if (playerSpeed == 0f)
                playerSpeed = 2;

            moveDirection.y = jumpAnimCurve.Evaluate(0);  //animation curve sets jump force
            canJump = true;  //starts jumpHandler
        }
        else //grounded but not jumping
        {
            if (!canJump)
                moveDirection.y = 0;  //if payer is grounded and not jumping, y is constrained
        }
    }


    void groundingCheck()
    {
        groundingRay = new Vector3(0, -2, 0);  //raycast to determine if player is grounded

        //grounded ray!
        if (Physics.Raycast(raycaster.transform.position, Vector3.down, out RaycastHit groundHit, 1f, groundLayer, previewConditionGround, 2f, Color.green, Color.red))
        {
            groundedPlayer = true;
        }
        else
        {
            groundedPlayer = false;

        }
    }
}


