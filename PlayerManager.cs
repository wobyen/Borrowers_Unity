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
        //climbDetector = GetComponent<ClimbDetector>();
        // hangingMechanics = GetComponent<HangingMechanics>();

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

                playerMovement.defaultMovement();

                break;

            case PlayerState.PullNPush:

                pushObjects.PushingObject(pushableRB);

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


    private void OnControllerColliderHit(ControllerColliderHit hit)  //if pl;ayer touches something that can be pushed
    {

        pushableRB = hit.collider.attachedRigidbody;
        animator.Play("Push Start");


        if (pushableRB != null && useAction.triggered)
        {

            ChangeState(PlayerState.PullNPush);
            Debug.Log("Can push");

        }



    }

}
