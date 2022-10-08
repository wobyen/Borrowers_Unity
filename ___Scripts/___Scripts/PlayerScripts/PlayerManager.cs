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

    AimHandler aimHandler;

    Rigidbody pushableRB;

    public Rig climbing;

    ClimbableDetection climableDetection;

    ClimbingHandler climbingHandler;

    public GameObject aimCube;

    [SerializeField]
    public enum PlayerState
    {
        BasicMovement,
        Aiming,
        Hanging,
        Climbing,
        PullNPush,
        ClimbSearch

    }


    private void Awake()
    {
        animator = GetComponent<Animator>();

        pushObjects = GetComponent<PushObjects>();

        playerControls = new PlayerControls();

        playerMovement = GetComponent<Movement>();

        gravityHandler = GetComponent<GravityHandler>();

        jumpHandler = GetComponent<JumpHandler>();

        climableDetection = GetComponent<ClimbableDetection>();

        aimHandler = GetComponent<AimHandler>();

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
                gravityHandler.GravityControls(-9);
                jumpHandler.JumpMechanics();
                climableDetection.DetectClimbNode();
                aimHandler.Aiming();


                break;



            case PlayerState.Hanging:

                gravityHandler.GravityControls(0);
                break;


            case PlayerState.ClimbSearch:

                climbing.weight = Mathf.Lerp(0, 1, Time.deltaTime);
                climableDetection.SearchForNextClimbPoint();
                gravityHandler.GravityControls(0);
                //  climbing.weight = 1;
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
