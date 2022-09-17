using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbingMechanics : MonoBehaviour
{

    [SerializeField] Transform climbingLandingRay;

    [SerializeField] Transform climbingRay;

    RaycastHit landingClimbHit;
    RaycastHit forwardHit;

    public LayerMask climbingLayer;

    // private PlayerMovement playerMovement;

    Animator _animator;
    PlayerInput _playerInput;

    PlayerMovement _playerMovement;
    CharacterController _controller;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction attackAction;
    private InputAction aimAction;






    private void Awake()
    {
        //playerMovement = PlayerMovement.Instance;


        _playerInput = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();

        //moveAction = playerInput.actions["Move"];
        //  jumpAction = _playerInput.actions["Jump"];
        //sprintAction = playerInput.actions["Sprint"];
        //attackAction = playerInput.actions["Attack"];
        //aimAction = playerInput.actions["Aim"];

    }




    public void CheckHang()  //checks if player is standing under or over a hanging volume
    {


        Vector3 upwards = transform.TransformDirection(Vector3.up) * 5;  //where the raycast point where look up and forwards for climbing

        Vector3 inFront = transform.TransformDirection(Vector3.forward) * 4;


        //  _playerMovement.canClimb = true;

        Physics.Raycast(climbingRay.transform.position, inFront, out forwardHit);

        //IF Player hits the jump key when in a CANHANG


        if (jumpAction.IsPressed())  //player chooses to climb 
        {

            _animator.SetBool("climbing", true);
            // newPosition = transform.position + new Vector3(0, climbHit.distance, forwardHit.distance);

            //  Vector3 hangPosition = new Vector3(landingClimbHit.point.x, landingClimbHit.point.y - _playerMovement.playerHeight, landingClimbHit.point.z - _playerMovement.playerWidth);

            //   Vector3 newPosition = new Vector3(landingClimbHit.point.x, landingClimbHit.point.y + (_playerMovement.playerHeight * 3), landingClimbHit.point.z + _playerMovement.playerWidth);

            //  Debug.Log("New position set to " + newPosition);


            //  _playerMovement.move.y = 1;  //make player ungrounded
            _animator.SetBool("grounded", false);

            // _playerMovement.hanging = true;

            return;

        }


        else
        {
            Debug.DrawRay(climbingRay.transform.position, upwards, Color.red);
            // _playerMovement.canClimb = false;

            Debug.DrawRay(climbingRay.transform.position, inFront, Color.red);


        }
        return;
    }



    public void Climb(InputAction.CallbackContext ctx)
    {

        Debug.Log("Hello!  This is marv.");

    }


}



