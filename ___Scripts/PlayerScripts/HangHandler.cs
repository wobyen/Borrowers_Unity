using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;


public class HangHandler : MonoBehaviour
{
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
    float hangMoveSpeed = 3;

    Vector3 lastValidHangPoint;

    //------------//--INPUTS--//------------//

    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;
    InputAction crouchAction;

    //------------//----//------------//

    public GameObject raycastClimb;

    public LayerMask ledgelayer;
    public PreviewCondition previewClimb;


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

    public void HangMovement()

    {

        //This raycast detects climable objects, grounds the player to a ledge while hanging and moving, and many other things.
        if (Physics.Raycast(raycastClimb.transform.position, -transform.up, out RaycastHit ledgeHit, 3f, ledgelayer, previewClimb, 1f, Color.green, Color.red))
        {

            Vector2 hangMoveInput = moveAction.ReadValue<Vector2>();

            Vector3 hangMovement = new Vector3(hangMoveInput.x, 0, 0);

            Vector3 localHangMovement = transform.TransformDirection(hangMovement);

            //giving player control to move side to side
            controller.Move(localHangMovement.x * transform.right * hangMoveSpeed * Time.deltaTime);

            lastValidHangPoint = transform.position;  //last valid point saved in case character moves off edge

        }

        else
        {
            transform.position += lastValidHangPoint;
        }




    }
}