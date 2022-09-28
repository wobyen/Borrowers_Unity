using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;


public class HangHandler : MonoBehaviour
{

    [Header("CACHED")]

    CharacterController controller;
    PlayerControls playerControls;
    JumpHandler jumphandler;
    Animator animator;
    LedgeHandler ledgeHandler;
    PlayerManager playerManager;

    ClimbSearch climbSearch;
    float hangMoveSpeed = 3;

    public Vector3 lastValidHangPoint;

    public Vector3 landingZone;

    //------------//--INPUTS--//------------//

    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;
    InputAction crouchAction;

    //------------//----//------------//

    public GameObject raycastClimb;
    public LayerMask ledgelayer;
    public PreviewCondition previewClimb;
    public List<Collider> ClimbPoints;


    private void Awake()
    {
        playerControls = new PlayerControls();

        controller = GetComponent<CharacterController>();

        playerManager = GetComponent<PlayerManager>();

        ledgeHandler = GetComponent<LedgeHandler>();

        climbSearch = GetComponent<ClimbSearch>();

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
    }


    private void OnDisable()
    {
        moveAction.Disable();

        jumpAction.Disable();

        sprintAction.Disable();

        crouchAction.Disable();
    }


    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        jumphandler = GetComponent<JumpHandler>();
        animator = GetComponent<Animator>();

        ClimbPoints = new List<Collider>();

    }


    public void HangMovement(Collider ledgeHit)  //can player do hang movement AND WHETHER ITS ONTO SOMETHING OR STARTING CLIMB SEQUENCE

    {
        if (ledgeHit.GetComponent<Collider>().gameObject.layer == 6) //can player move left and right and climb up?
        {
            //This raycast detects climable objects, grounds the player to a ledge while hanging and moving, and many other things.
            if (Physics.Raycast(raycastClimb.transform.position, -transform.up, out RaycastHit hangGroundHit, 3f, ledgelayer, previewClimb, 1f, Color.green, Color.red))
            {

                Vector2 hangMoveInput = moveAction.ReadValue<Vector2>();
                Vector3 hangMovement = new Vector3(hangMoveInput.x, 0, 0);

                lastValidHangPoint = transform.position;  //last valid point saved in case character moves off edge

                //giving player control to move side to side
                controller.Move(hangMovement.x * transform.right * hangMoveSpeed * Time.deltaTime);

                landingZone = hangGroundHit.point;
            }
            else
            {
                transform.position = lastValidHangPoint;
            }

            if (jumpAction.IsPressed())  //player is climbing up
            {
                StartCoroutine(ClimbLedge(landingZone));
            }
        }


        else if (ledgeHit.GetComponent<Collider>().gameObject.layer == 7)
        {
            Debug.Log("This is a climb point.");

            playerManager.ChangeState(PlayerManager.PlayerState.ClimbSearch);

        }


        if (crouchAction.IsPressed())
        {
            //player drops from ledge

            animator.SetBool("isDropping", true);
            playerManager.ChangeState(PlayerManager.PlayerState.BasicMovement);
            animator.SetBool("isHanging", false);
            animator.SetBool("isClimbing", false);

        }

    }


    public IEnumerator ClimbLedge(Vector3 landingZone)  //climbing on top of obstacle at end of Climb seqwuence or block
    {


        animator.SetBool("isClimbing", true);

        transform.DOMoveY(landingZone.y, 1.0f);  //player moves up to ledge height

        yield return new WaitForSeconds(1.0f);  //waits for animation to complete

        transform.DOMove(landingZone, .5f);  //player moves forward onto block

        yield return null;

        animator.SetBool("isHanging", false);
        animator.SetBool("isClimbing", false);

        yield return null;

        playerManager.ChangeState(PlayerManager.PlayerState.BasicMovement);  //return control back to player

    }




}