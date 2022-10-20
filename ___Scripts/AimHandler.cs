using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;
using UnityEngine.Animations.Rigging;



public class AimHandler : MonoBehaviour
{

    [SerializeField] CinemachineFreeLook aimCam;

    [SerializeField] GameObject aimCube;
    [SerializeField] GameObject aimCast;


    CharacterController controller;
    PlayerControls playerControls;
    Movement playerMovement;
    GravityHandler gravityHandler;
    LedgeHandler LedgeHandler;

    Animator animator;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;
    InputAction crouchAction;

    InputAction attackAction;

    InputAction aimAction;

    [SerializeField] Rig aimRig;

    [SerializeField] GameObject grappleIcon;




    private void Awake()
    {
        playerControls = new PlayerControls();

        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<Movement>();

        animator = GetComponent<Animator>();
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

        aimAction = playerControls.Player.Aim;
        aimAction.Enable();

        attackAction = playerControls.Player.Attack;
        attackAction.Enable();
    }


    private void OnDisable()
    {
        moveAction.Disable();

        jumpAction.Disable();

        sprintAction.Disable();

        crouchAction.Disable();

        aimAction.Disable();
    }


    public void Aiming()
    {

        if (aimAction.IsPressed())
        {
            SetAimCamera(100);

            AimToggles(true);


        }

        else
        {
            SetAimCamera(0);

            AimToggles(false);

        }

    }


    public void AimToggles(bool b_aim)
    {
        animator.SetBool("isAiming", b_aim);
        aimCube.gameObject.SetActive(b_aim);


        if (b_aim)
        {
            Vector3 aimDir = aimCast.transform.position - aimCube.transform.position;
            aimRig.weight = 100;


            if (Physics.Raycast(aimCast.transform.position, -aimDir, out RaycastHit grappleHit, Mathf.Infinity, PreviewCondition.Both, 1f, Color.green, Color.red))
            {

                // grappleIcon.transform.position = grappleHit.point;



                if (attackAction.triggered)
                {
                    transform.DOMove(grappleHit.point, 1f).SetEase(Ease.InOutQuint);

                }
            }
        }

        else
        {
            aimRig.weight = 0;

        }

    }






    public void SetAimCamera(int priority)
    {

        aimCam.Priority = priority;

    }



}