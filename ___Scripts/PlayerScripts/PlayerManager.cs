using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{


    Movement playerMovement;
    //[SerializeField] ClimbDetector climbDetector;
    Animator animator;
    InputAction useAction;
    InputAction moveAction;

    PlayerControls playerControls;
    PushObjects pushObjects;
    //HangingMechanics hangingMechanics;

    ClimbingHandler climbingHandler;

    CharacterController controller;

    GravityHandler gravityHandler;
    JumpHandler jumpHandler;

    HangHandler hangHandler;

    Rigidbody pushableRB;



    [SerializeField]
    public enum PlayerState
    {
        BasicMovement,
        Hanging,
        Climbing,
        PullNPush

    }
    private void Awake()
    {
        animator = GetComponent<Animator>();

        pushObjects = GetComponent<PushObjects>();

        climbingHandler = GetComponent<ClimbingHandler>();

        playerControls = new PlayerControls();

        playerMovement = GetComponent<Movement>();

        gravityHandler = GetComponent<GravityHandler>();

        jumpHandler = GetComponent<JumpHandler>();

        hangHandler = GetComponent<HangHandler>();

    }




    private void OnEnable()
    {

        useAction = playerControls.Player.Use;
        useAction.Enable();

    }

    private void OnDisable()
    {
        useAction.Disable();
    }




    public PlayerState state;

    private void Start()
    {
        state = PlayerState.BasicMovement;
    }


    private void PlayerStateChange(PlayerState state)
    {

        switch (state)
        {
            //basic movement when player is on the ground, no special abilities
            case PlayerState.BasicMovement:

                playerMovement.defaultMovement(); //walking and jumping
                climbingHandler.ClimbingMechanics();
                gravityHandler.GravityControls(-9);
                jumpHandler.JumpMechanics();

                break;


            case PlayerState.Climbing:

                climbingHandler.ClimbingMechanics();
                gravityHandler.GravityControls(0);
                break;



            case PlayerState.Hanging:

                hangHandler.HangMovement();
                gravityHandler.GravityControls(0);
                break;

        }
    }

    private void FixedUpdate()
    {

        PlayerStateChange(state);

    }


    public void ChangeState(PlayerState newState)
    {
        // Debug.Log("Changing Player State");
        state = newState;

    }




}
