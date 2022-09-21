using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{


    [SerializeField] PlayerMovement playerMovement;
    //[SerializeField] ClimbDetector climbDetector;
    Animator animator;
    InputAction useAction;
    InputAction moveAction;

    PlayerControls playerControls;
    PushObjects pushObjects;
    //HangingMechanics hangingMechanics;

    ClimbingHandler climbingHandler;

    CharacterController controller;

    Rigidbody pushableRB;


    [SerializeField]
    public enum PlayerState
    {
        BasicMovement,
        Hanging,
        Climbing,
        Gliding,
        PullNPush,
        JumpJets,
        GrapplingHook,
        ScrewDriver,
        Thumbtacks


    }
    private void Awake()
    {
        animator = GetComponent<Animator>();

        pushObjects = GetComponent<PushObjects>();

        climbingHandler = GetComponent<ClimbingHandler>();

        playerControls = new PlayerControls();

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
                pushObjects.PushingObject();  //detects if player can push
                climbingHandler.ClimbingMechanics();


                break;

            case PlayerState.PullNPush:

                pushObjects.PushingObject(); //pushing object functions

                break;


            case PlayerState.Climbing:

                climbingHandler.ClimbingMechanics();

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
