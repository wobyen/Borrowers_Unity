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

    PlayerControls playerControls;
    PushObjects pushObjects;
    //HangingMechanics hangingMechanics;
    GravityHandler gravityHandler;
    JumpHandler jumpHandler;

    AimHandler aimHandler;

    public Rig climbing;

    ClimbableDetection climableDetection;

    [SerializeField]
    public enum PlayerState
    {
        BasicMovement,
        Aiming,
        Hanging,
        Climbing,
        PullNPush,
        FreeClimb,
        NoInput
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
                climableDetection.StartClimb();
                aimHandler.Aiming();

                break;


            case PlayerState.FreeClimb:

                climableDetection.FreeClimb();
                gravityHandler.GravityControls(0);


                break;


            case PlayerState.NoInput:
                gravityHandler.GravityControls(-9);

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
