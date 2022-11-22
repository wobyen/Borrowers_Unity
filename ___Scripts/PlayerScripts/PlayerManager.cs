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

    PlayerControls playerControls;
    PushObjects pushObjects;
    //HangingMechanics hangingMechanics;
    GravityHandler gravityHandler;
    JumpHandler jumpHandler;
    AimHandler aimHandler;
    ClimbableDetection climableDetection;
    HangHandler hangHandler;

    public bool isDisguised = false;

    [SerializeField]
    public enum PlayerState
    {
        BasicMovement,
        FixedCamMovement,
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

        hangHandler = GetComponent<HangHandler>();

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

                playerMovement.defaultMovement();
                playerMovement.playerRotationCamera();
                //walking and jumping
                gravityHandler.GravityControls(-9);
                jumpHandler.JumpMechanics();
                climableDetection.StartClimb();
                hangHandler.JumpToHang();

                aimHandler.Aiming();

                break;




            case PlayerState.FreeClimb:

                climableDetection.FreeClimb();
                gravityHandler.GravityControls(0);


                break;


            case PlayerState.NoInput:
                gravityHandler.GravityControls(-9);

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
