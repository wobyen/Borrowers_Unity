

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;
public class ClimbSearch : MonoBehaviour
{
    // Start is called before the first frame update

    public float searchSphereSize = .5f;
    public float searchSphereRange = 2.5f;


    [Header("References")]
    Animator animator;
    PlayerManager playerManager;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction crouchAction;
    PlayerControls playerControls;
    CharacterController controller;

    HangHandler hangHandler;
    JumpHandler jumpHandler;

    Vector3 rotateSearch;


    //VECTORS

    Vector3 climbSearchInput;


    [Header("External Links")]

    public bool topLedge;
    public LayerMask climbSearchLayer;
    public PreviewCondition previewClimb;
    public LayerMask climbLayer;
    public LedgeHandler ledgeHandler;

    public Collider ledgeCollider;

    public Collider[] results;
    public List<Collider> ClimbPoints;
    public GameObject orbitSphere;

    private void OnEnable()
    {
        moveAction = playerControls.Player.Move;

        moveAction.Enable();

        jumpAction = playerControls.Player.Jump;
        jumpAction.Enable();

        crouchAction = playerControls.Player.Crouch;
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
    }



    private void Start()
    {
        previewClimb = PreviewCondition.Both;

        playerManager = GetComponent<PlayerManager>();

        animator = GetComponent<Animator>();

        ledgeHandler = GetComponent<LedgeHandler>();

        hangHandler = GetComponent<HangHandler>();
    }


    public void NextClimbPoint()

    {

        //controls for player to search for next climb point
        climbSearchInput = moveAction.ReadValue<Vector2>();
        Vector3 climbNormInput = climbSearchInput.normalized;

        orbitSphere.transform.localPosition = new Vector2(climbSearchInput.x * searchSphereRange, climbSearchInput.y * searchSphereRange);

        //the array where the climb points are stored
        results = Physics.OverlapSphere(orbitSphere.transform.position, searchSphereSize, climbSearchLayer, PreviewCondition.Both, .05f, Color.green, Color.red);


        if (jumpAction.IsPressed())
        {

            if (results[0] != null)
            {

                Vector3 grabPoint = results[0].transform.position;
                StartCoroutine("ClimbPointChosen", grabPoint);

            }
            else
            {
                Debug.Log("No Climbpoints!");
            }
        }

        if (crouchAction.IsPressed())
        {
            //player drops from ledge

            ledgeHandler.canClimb = false;
            animator.SetBool("isDropping", true);
            playerManager.ChangeState(PlayerManager.PlayerState.BasicMovement);
            animator.SetBool("isHanging", false);
            animator.SetBool("isClimbing", false);

        }
    }

    public IEnumerator ClimbPointChosen(Vector3 grabPoint)
    {

        animator.SetBool("isHangJumping", true);

        Vector3 ledgePositionAdjusted = new Vector3(grabPoint.x, grabPoint.y - controller.height * .85f, grabPoint.z);

        animator.SetBool("isHanging", true);

        // yield return new WaitForSeconds(.13f);  //wait for animation

        transform.DOMove(ledgePositionAdjusted, .75f).SetEase(Ease.InOutQuad);


        if (results[0].gameObject.CompareTag("topLedge"))
        {
            Debug.Log("TOP!");

            StartCoroutine(hangHandler.ClimbLedge(grabPoint));

        }

        yield return new WaitForSeconds(1);

        animator.SetBool("isHangJumping", false);

    }



}
