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
    public bool isJumping = false;


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

    public Vector3 jumpHeight;

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

            // PlayerVelocity();

            //adjusts moveDirection normals to reorient the player for the camera correctly
            moveDirection = moveDirection.x * Camera.main.transform.right.normalized + moveDirection.z * Camera.main.transform.forward.normalized;

            jumpHeight.y = 0;
            moveDirection.y = 0;

        }
        else //not grounded //fallling
        {

            Debug.Log("Applying gravity!");

            gravity = -9.8f;
            jumpHeight.y = 1f;

            animator.SetBool("isFalling", true);
            animator.SetBool("isGrounded", false);  //animations when player is on the ground

            controller.Move(jumpHeight * gravity * Time.deltaTime);

            //if player is not grounded, they are falling.
        }


        JumpHandler();  // while grounded, if isJumping = true, how does the jump work?

        controller.Move(moveDirection * playerSpeed * Time.deltaTime);  // the input moves the player

        //rotate the player with camera
        Quaternion targetRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);   // find the camera's Y rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // animate the rotation with LERP
                                                                                                                  //controller.Move(moveDirection * playerSpeed * Time.deltaTime);
    }


    //------
    //------   METHODS
    //------


    void JumpHandler()  //how the jump works
    {

        if (jumpAction.triggered && groundedPlayer && !isJumping)  //if jump is pressed and jump is over
        {
            animator.SetBool("isJumping", true);

            jumpTime = 0;    //reset jump timer
            //moveDirection.y = jumpAnimCurve.Evaluate(0);  //animation curve sets jump force
            isJumping = true;  //starts jumpHandler
        }

        else
        {
            jumpHeight.y = 1;
        }

        if (isJumping)
        {
            jumpTime += Time.deltaTime;  //jump timer start

            gravity = -1f;
            jumpHeight = new Vector3(0, jumpAnimCurve.Evaluate(jumpTime), 0);

            transform.Translate(jumpHeight, Space.Self);  //ACTUAL JUMP

            if (jumpTime > .5f)  //time for jump to complete
            {
                isJumping = false;
            }

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


    void PlayerVelocity()
    {
        if (moveDirection != Vector3.zero)   //storing momentum data
        {
            lastDirection = moveDirection;
            storedVelocity = playerSpeed;
        }

        if (moveInput == Vector2.zero)   //if player stops moving 
        {
            lastDirection.y = 0;
            controller.Move(lastDirection * storedVelocity * Time.deltaTime);

            playerSpeed = storedVelocity;  // put last spoeed value in stored velocity

            storedVelocity -= slowRate * Time.deltaTime;  //slowly remove velocity

            animator.Play("Walking");
            animator.speed = storedVelocity;

            if (storedVelocity < 0)
            {
                storedVelocity = 0;
            }

        }
        else
        {
            animator.speed = 1;
        }
    }


}


