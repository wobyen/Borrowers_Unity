using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
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

    LedgeHandler LedgeHandler;

    CharacterController controller;

    GravityHandler gravityHandler;
    JumpHandler jumpHandler;

    HangHandler hangHandler;

    ClimbSearch climbSearch;
    Rigidbody pushableRB;

    public Rig climbing;

    ClimbingHandler climbingHandler;

    [SerializeField]
    public enum PlayerState
    {
        BasicMovement,
        Hanging,
        Climbing,
        PullNPush,
        ClimbSearch


    }



    private void Awake()
    {
        animator = GetComponent<Animator>();

        pushObjects = GetComponent<PushObjects>();

        LedgeHandler = GetComponent<LedgeHandler>();

        playerControls = new PlayerControls();

        playerMovement = GetComponent<Movement>();

        gravityHandler = GetComponent<GravityHandler>();

        jumpHandler = GetComponent<JumpHandler>();

        hangHandler = GetComponent<HangHandler>();

        climbingHandler = GetComponent<ClimbingHandler>();

        climbSearch = GetComponent<ClimbSearch>();
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
                LedgeHandler.LedgeMechanics();
                gravityHandler.GravityControls(-9);
                jumpHandler.JumpMechanics();
                climbing.weight = 0;


                break;


            case PlayerState.Climbing:

                LedgeHandler.LedgeMechanics();
                gravityHandler.GravityControls(0);

                break;


            case PlayerState.Hanging:

                hangHandler.HangMovement(LedgeHandler.ledgeCollider);
                gravityHandler.GravityControls(0);
                break;


            case PlayerState.ClimbSearch:

                climbSearch.NextClimbPoint();
                gravityHandler.GravityControls(0);
                climbing.weight = 1;
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
