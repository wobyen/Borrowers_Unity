using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;


public class HangHandler : InputManager
{

    [Header("CACHED")]

    CharacterController controller;
    PlayerControls playerControls;
    JumpHandler jumphandler;
    Animator animator;
    LedgeHandler ledgeHandler;
    PlayerManager playerManager;

    float hangMoveSpeed = 3;

    public Vector3 lastValidHangPoint;

    public Vector3 landingZone;

    GroundCheck groundCheck;

    bool hanging;

    [SerializeField] GameObject raycastForward;
    [SerializeField] GameObject raycastAbove;
    [SerializeField] GameObject raycastAboveAhead;


    //------------//----//------------//

    public GameObject raycastClimb;
    public LayerMask ledgelayer;
    public PreviewCondition previewClimb;


    private void Awake()
    {
        playerControls = new PlayerControls();

        controller = GetComponent<CharacterController>();

        playerManager = GetComponent<PlayerManager>();

    }



    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        jumphandler = GetComponent<JumpHandler>();
        animator = GetComponent<Animator>();
        groundCheck = GetComponent<GroundCheck>();

    }


    public void JumpToHang()
    {

        if (!groundCheck.isGrounded())

        {
            Debug.Log("is this working");

            //detects if there is a ledge ahead
            if (!hanging && Physics.Raycast(raycastForward.transform.position, transform.forward, out RaycastHit ledgeHit, 1f, previewClimb, 1f, Color.green, Color.red) && Physics.Raycast(raycastAboveAhead.transform.position, -transform.up, out RaycastHit climbUpPoint, .75f, previewClimb, 2f, Color.green, Color.yellow))
            {

                hanging = true;
                animator.Play("Stand To Freehang");


                lastValidHangPoint = new Vector3(ledgeHit.point.x, ledgeHit.point.y, ledgeHit.point.z);

                // transform.DOMove(lastValidHangPoint, 1f);

                playerManager.ChangeState(PlayerManager.PlayerState.Hanging);
            }
        }
    }

    public void HangMovement()  //can player do hang movement AND WHETHER ITS ONTO SOMETHING OR STARTING CLIMB SEQUENCE

    {
        //This raycast detects climable objects, grounds the player to a ledge while hanging and moving, and many other things.
        if (Physics.Raycast(raycastClimb.transform.position, -transform.up, out RaycastHit hangGroundHit, 3f, previewClimb, 1f, Color.green, Color.red))
        {

            Vector2 hangMoveInput = moveAction.ReadValue<Vector2>();
            Vector3 hangMovement = new Vector3(hangMoveInput.x, 0, 0);

            lastValidHangPoint = transform.position;  //last valid point saved in case character moves off edge

            //giving player control to move side to side
            controller.Move(hangMovement.x * transform.right * hangMoveSpeed * Time.deltaTime);

            landingZone = hangGroundHit.point;

            if (Physics.Raycast(raycastForward.transform.position, transform.forward, out RaycastHit ledgeHit, 2f, ledgelayer, previewClimb, 1f, Color.green, Color.red))

            {
                Vector3 surfaceNormal = -ledgeHit.normal;

                if (ledgeHit.distance > .40f)  //iof player drifts too far away, they come back to wall
                {
                    transform.position += surfaceNormal * 2 * Time.deltaTime;
                }

                else if (ledgeHit.distance < .35f)  //if they start to clip into wall, they go back to wall
                {
                    transform.position -= surfaceNormal * 2 * Time.deltaTime;
                }
            }
        }
        else
        {
            transform.position = lastValidHangPoint;
        }

        if (jumpAction.IsPressed())  //player is climbing up
        {
            StartCoroutine(ClimbLedge(landingZone));
        }

        if (crouchAction.IsPressed())
        {
            //player drops from ledge
            animator.SetBool("isDropping", true);
            animator.SetBool("isHanging", false);
            animator.SetBool("isClimbing", false);
            hanging = false;
            playerManager.ChangeState(PlayerManager.PlayerState.BasicMovement);


        }

    }


    public IEnumerator ClimbLedge(Vector3 landingZone)  //climbing on top of obstacle at end of Climb seqwuence or block
    {


        animator.Play("climbing up");

        animator.SetBool("isClimbing", true);

        transform.DOMoveY(landingZone.y, 1.0f);  //player moves up to ledge height

        yield return new WaitForSeconds(1.0f);  //waits for animation to complete

        transform.DOMove(landingZone, .5f);  //player moves forward onto block

        yield return null;

        animator.SetBool("isHanging", false);
        animator.SetBool("isClimbing", false);

        yield return null;
        hanging = false;

        playerManager.ChangeState(PlayerManager.PlayerState.BasicMovement);  //return control back to player

    }




}