using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;
using TMPro;
using DG.Tweening;
using UnityEngine.Animations.Rigging;

public class ClimbableDetection : MonoBehaviour
{

    //-----// RAYCASTS //-----//

    [SerializeField] GameObject raycastClimb;
    [SerializeField] GameObject raycastForward;

    [SerializeField] GameObject raycastHangMove;
    [SerializeField] GameObject raycastLedge;

    [SerializeField] LayerMask ledgelayer;
    [SerializeField] LayerMask climbSearchLayer;

    [SerializeField] PreviewCondition previewClimb;

    [SerializeField] GameObject orbitSphere;

    [SerializeField] float searchSphereRange;
    [SerializeField] float searchSphereSize;

    //-----// COMPONENTS //-----//

    Animator animator;
    PlayerManager playerManager;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction crouchAction;
    PlayerControls playerControls;
    CharacterController controller;
    JumpHandler jumpHandler;
    ClimbSearch climbSearch;
    public Rig climbing;
    float lerpAmount;

    Vector3 towardsLedge;

    bool climbSearchInProcess;

    public bool canClimb = false;

    //-----// BOOLS //------//


    public bool climbNodeSequence;

    //-----// ARRAY //------//

    public Collider[] results;

    Vector3 lastValidHangPoint;

    [SerializeField] float climbSpeed;
    [SerializeField] float inverseHangInput;

    private void OnEnable()
    {
        moveAction = playerControls.Player.Move;

        moveAction.Enable();

        jumpAction = playerControls.Player.Jump;
        jumpAction.Enable();

        crouchAction = playerControls.Player.Crouch;

        crouchAction.performed += ctx => StartCoroutine(ClimbCancel());


        crouchAction.Enable();
    }


    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        crouchAction.Disable();
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
        controller = GetComponent<CharacterController>();
        jumpHandler = GetComponent<JumpHandler>();
        playerManager = GetComponent<PlayerManager>();
        animator = GetComponent<Animator>();


        climbing.weight = 0;
    }


    public void DetectClimbNode()   //searching for climable nodes or ledges
    {
        //This raycast detects climable objects, grounds the player to a ledge while hanging and moving, and many other things.
        if (Physics.Raycast(raycastClimb.transform.position, -transform.up, out RaycastHit ledgeHit, 3f, ledgelayer, previewClimb, 1f, Color.green, Color.red))
        {
            canClimb = true;

            Vector3 ledgeLocation = ledgeHit.point;  //find the Vector3 of the climb point

            if (jumpAction.IsPressed() && jumpHandler.playerGrounded)  //if jump is pressed annd player is grounded AND raycast positive
            {
                towardsLedge = OrientPlayer();  //make sure player is perpindicular to surface
                transform.forward = -towardsLedge;

                animator.SetBool("startClimb", true);  //start animation of player -- IDLE to HANG


                // if (ledgeHit.collider.CompareTag("finishClimb"))
                // {
                //     StartCoroutine(FinishClimb(ledgeLocation));
                // }

                // else
                // {
                //Search for climb nodes while hanging
                StartCoroutine(ClimbingNodeSelected(ledgeHit.point));  //player jumps up to grab ledge

                playerManager.ChangeState(PlayerManager.PlayerState.ClimbSearch);  //start node search state
                // }
            }
        }
        else
        {
            canClimb = false;
        }
    }

    public IEnumerator ClimbingNodeSelected(Vector3 grabPoint)  //player jumps up to grab ledge
    {
        //animator.SetBool("isClimbing", true);  //animate player jumping from point to point

        Vector3 ledgePositionAdjusted = new Vector3(grabPoint.x, grabPoint.y - controller.height * .85f, grabPoint.z); //adjust vec3 for player height

        // yield return new WaitForSeconds(.13f);  //wait for animation
        transform.DOMove(ledgePositionAdjusted, .75f).SetEase(Ease.InOutQuad);  //player actually leaps to point

        if (results[0] != null && results[0].gameObject.CompareTag("finishClimb"))  //if the element is tagged as the top, skip to the end    

        {
            StartCoroutine(FinishClimb(grabPoint)); //climb on top
        }

        yield return new WaitForSeconds(.5f);
        animator.SetBool("isClimbing", false);  //emd animation of jump

    }

    public void SearchForNextClimbPoint()  //main loop of climb Search player state

    {
        lerpAmount += Time.deltaTime;
        climbing.weight = Mathf.Lerp(0, 1, lerpAmount); //animationm rig activates

        Vector3 climbSearchInput = moveAction.ReadValue<Vector2>();
        Vector3 climbNormInput = climbSearchInput.normalized;  //input for hang movement

        orbitSphere.transform.localPosition = new Vector2(climbSearchInput.x * searchSphereRange, climbSearchInput.y * searchSphereRange);  //find next ledge

        //the array where the climb points are stored
        results = Physics.OverlapSphere(orbitSphere.transform.position, searchSphereSize, climbSearchLayer, PreviewCondition.Both, .05f, Color.green, Color.red);

        lastValidHangPoint = transform.position;  //last valid point saved in case character moves off edge

        //This raycast detects climable objects, grounds the player to a ledge while hanging and moving, and many other things.
        if (Physics.Raycast(raycastHangMove.transform.position, -transform.up, out RaycastHit hangGroundHit, 1f, ledgelayer, previewClimb, 1f, Color.green, Color.red))
        {
            //lastValidHangPoint = transform.position;
            inverseHangInput = climbSearchInput.x;

            //giving player control to move side to side
            controller.Move(climbSearchInput.x * transform.right * climbSpeed * Time.deltaTime);
            animator.SetFloat("hangMove", climbSearchInput.x);

        }
        else  //don't allow player to hang move beyond ledge edge
        {
            transform.position += -inverseHangInput * transform.right * climbSpeed * Time.deltaTime;
            // transform.position = lastValidHangPoint;
        }


        if (jumpAction.IsPressed())
        {
            if (results[0] != null)
            {
                Vector3 grabPoint = results[0].transform.position;
                animator.SetBool("isClimbing", true);  //animate player jumping from point to point

                StartCoroutine("ClimbingNodeSelected", grabPoint);
            }
            else
            {
                Debug.Log("No Climbpoints!");
            }
        }


    }



    public IEnumerator FinishClimb(Vector3 landingZone)  //climbing on top of obstacle at end of Climb seqwuence or block
    {
        animator.SetBool("isClimbing", false);
        animator.SetBool("finishClimb", true);

        climbing.weight = 0;
        lerpAmount = 0;

        Vector3 landingZoneAdjusted = new Vector3(landingZone.x, landingZone.y - controller.height * .5f, landingZone.z);


        transform.DOMoveY(landingZoneAdjusted.y, 1.0f);  //player moves up to ledge height

        yield return new WaitForSeconds(1.0f);  //waits for animation to complete

        transform.DOMove(landingZoneAdjusted + -towardsLedge, .5f);  //player moves forward onto block

        //transform.position += (towardsLedge - transform.position * 6);

        yield return null;


        animator.SetBool("startClimb", false);
        animator.SetBool("finishClimb", false);
        //return control back to player
        playerManager.ChangeState(PlayerManager.PlayerState.BasicMovement);

    }


    public IEnumerator ClimbCancel()  //input from crouch button
    {
        animator.SetBool("isDropping", true);

        playerManager.ChangeState(PlayerManager.PlayerState.BasicMovement);
        climbing.weight = 0;

        //player drops from ledge

        yield return null;

        animator.SetBool("startClimb", false);
        animator.SetBool("finishClimb", false);

        yield return new WaitForSeconds(1);

        animator.SetBool("isClimbing", false);
        animator.SetBool("isDropping", false);
    }

    public Vector3 OrientPlayer()
    {
        //ORIENT PLAYER
        if (Physics.Raycast(raycastForward.transform.position, transform.forward, out RaycastHit forwardHit, 2f, ledgelayer, previewClimb, 1f, Color.green, Color.red))
        {
            Debug.Log(forwardHit.normal);

            return forwardHit.normal;
        }
        else
        {
            return transform.forward;
        }
    }
}