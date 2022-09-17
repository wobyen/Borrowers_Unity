using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{


    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] ClimbDetector climbDetector;

    PushObjects pushObjects;
    //HangingMechanics hangingMechanics;


    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        pushObjects = GetComponent<PushObjects>();
        //climbDetector = GetComponent<ClimbDetector>();
        // hangingMechanics = GetComponent<HangingMechanics>();
    }




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

                break;

            case PlayerState.PullNPush:

                pushObjects.PushingObject();

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
