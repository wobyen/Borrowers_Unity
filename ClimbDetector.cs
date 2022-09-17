using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using DG.Tweening;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;


public class ClimbDetector : MonoBehaviour
{
    //-------------// REFERENCES  //-------------//

    [Header("References")]

    [SerializeField] private CharacterController controller;
    PlayerManager playerManager;


    PlayerControls playerControls;

    Animator animator;

    //----------//BOOLS//--------------//

    [Header("Bools")]


    public bool hangInProgress;

    public bool hanging;

    public bool playerCanLowerDown;
    public bool playerCanHang;


    //-------------//  CLIMBING    //-------------//

    [SerializeField] LayerMask ledgeLayer;
    RaycastHit ledgeSearchDownHit;
    [SerializeField] Transform ledgeSearchForwardRay;
    [SerializeField] Transform ledgeSearchDownRay;



    public float playerHeight = 1.8f;
    public float playerWidth = .28f;



    Vector3 hangPosition;

    float ledgeDistance;

    [SerializeField]

    public enum ClimbState
    {
        NoClimb = 0,
        CanClimb,
        Hanging,
        NoClimbingUp,
        ClimbingUp,
        DoneClimbing
    }
    [SerializeField] PreviewCondition previewConditionClimb;

    public ClimbState climbState;


    InputAction jumpAction;
    InputAction crouchAction;

    //-------------//       //-------------//


    private void OnEnable()
    {
        jumpAction = playerControls.Player.Jump;  //make alias for player controls
        jumpAction.Enable();   //enables the controls


        crouchAction = playerControls.Player.Crouch;  //make alias for player controls

        crouchAction.Enable();   //enables the controls

    }

    private void OnDisable()
    {
        jumpAction.Disable();


        crouchAction = playerControls.Player.Crouch;  //make alias for player controls

        crouchAction.Enable();
    }

    private void Awake()
    {

        playerControls = new PlayerControls();

        playerManager = GetComponent<PlayerManager>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        hangInProgress = false;

    }

    private void Start()
    {
        climbState = ClimbState.NoClimb;
    }

    // Update is called once per frame


    public void CanPlayerClimb()
    {
        // Debug.DrawRay(ledgeSearchForwardRay.transform.position, Vector3.forward, Color.red);
        Debug.DrawRay(ledgeSearchDownRay.transform.position, Vector3.down, Color.red);

        switch (climbState)
        {
            case ClimbState.NoClimb:

                if (Physics.Raycast(ledgeSearchDownRay.transform.position, Vector3.down, out ledgeSearchDownHit, Mathf.Infinity, ledgeLayer, previewConditionClimb, 2f, Color.green, Color.red)) //if raycast hits a climbing layer
                {
                    //Debug.DrawRay(ledgeSearchForwardRay.transform.position, Vector3.forward, Color.green);
                    Debug.DrawRay(ledgeSearchDownRay.transform.position, Vector3.down, Color.green);

                    if (ledgeSearchDownHit.point.y > transform.position.y)
                    {
                        playerCanHang = true;
                        playerCanLowerDown = false;
                    }
                    else if (ledgeSearchDownHit.point.y < transform.position.y)
                    {
                        playerCanLowerDown = true;
                        playerCanHang = false;
                    }



                    if (jumpAction.triggered && playerCanHang)  //if player junmps while under a climbing layer

                    {   //set thesse variables at the point the player triggers interaction
                        ledgeDistance = Vector3.Distance(transform.position, ledgeSearchDownHit.point);
                        //find height distance between player and ledge
                        hangPosition = new Vector3(transform.position.x, (transform.position.y + ledgeDistance) - playerHeight - .2f, transform.position.z - (playerWidth / 3));
                        //create new Vector 3 where player should be hanging from

                        Debug.Log("Player initiating climb! " + hangPosition);

                        ChangeState(ClimbState.ClimbingUp);
                        hanging = true;
                    }

                    if (crouchAction.triggered && playerCanLowerDown)
                    {
                        Debug.Log("Player is lowering themself into hang");
                    }
                }
                else
                {
                    playerCanHang = false;
                }
                break;


            case ClimbState.ClimbingUp:   //player jumps onto object and begins hang.

                playerManager.ChangeState(PlayerManager.PlayerState.Hanging);  //change player cointrols to hanging

                animator.SetBool("hanging", true);    //set animator to hanging

                //ANIMATOR EVENT //START HANG 

                break;


            case ClimbState.Hanging:  //player is hanging -- player can drop down or climb up

                if (jumpAction.triggered && hanging)  //player wants to climb up from hang
                {
                    animator.SetBool("hanging", false);
                    animator.SetBool("climbUp", true);

                    Debug.Log("Player starts climbing");
                }

                break;

            default:
                { }
                break;
        }
    }



    public void ChangeState(ClimbState newState)
    {
        // Debug.Log("Changing Climb state");
        climbState = newState;


    }


    public void StartHang()  //TRIGGERED BY ANIMATION EVENT 
    {
        // transform.DOLocalMove(hangPosition, 1, false);
        transform.DOMoveY(ledgeSearchDownHit.point.y - (playerHeight * .65f), .5f).SetEase(Ease.InOutCubic);

        ChangeState(ClimbState.Hanging);
    }


    public void ClimbUpAnimEvent()  //TRIGGERED BY ANIMATION EVENT 
    {
        transform.DOLocalMoveY(1, .5f).SetEase(Ease.InOutCubic);
    }


    IEnumerator FinishClimb()   //triggered from animation event (standing from hang)
    {

        float playerYMove = transform.position.y + 1.5f;
        transform.DOLocalMoveY(playerYMove, 1).SetEase(Ease.InOutCubic);  //move the player up with animation

        yield return new WaitForSeconds(.5f);

        transform.Translate(Vector3.forward * .5f);

        yield return new WaitForSeconds(.5f);

        hanging = false;
        animator.SetBool("climbUp", false);  // reset climb animation
        animator.SetBool("hanging", false);  // reset hanging animation

        playerManager.ChangeState(PlayerManager.PlayerState.BasicMovement); //give player controls back
        ChangeState(ClimbState.NoClimb);

    }



}
