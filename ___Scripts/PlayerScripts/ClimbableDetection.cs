using System.Collections;
using UnityEngine;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;
using DG.Tweening;
using UnityEngine.Animations.Rigging;

public class ClimbableDetection : InputManager
{
    public Vector3 climbDirection;
    public float cornerDirectionAmount = 3f;
    public float cornerFowardAmount = 5f;
    public bool corneringLeft = false;
    public bool corneringRight = false;
    public bool climbingUp = false;


    //-----// RAYCASTS //-----//
    [SerializeField] float climbRayLength;
    public float climbSpeed = 1f;
    GroundCheck groundCheck;

    public Vector3 surfaceNormal;
    public Vector3 previousNormal;

    [SerializeField] GameObject raycastForward;
    [SerializeField] GameObject raycastAbove;
    [SerializeField] GameObject raycastAboveAhead;


    [SerializeField] LayerMask ledgelayer;
    [SerializeField] LayerMask everythingLayer;
    [SerializeField] LayerMask nothingLayer;
    [SerializeField] PreviewCondition previewClimb;


    //-----// COMPONENTS //-----//

    Animator animator;
    PlayerManager playerManager;
    PlayerControls playerControls;
    CharacterController controller;
    JumpHandler jumpHandler;

    public bool canClimb = false;
    Vector3 lastValidHangPoint;
    BoxCollider climbCollider;

    float playerHeight;
    float playerRadius;



    private void Awake()
    {
        playerControls = new PlayerControls();
        controller = GetComponent<CharacterController>();
        jumpHandler = GetComponent<JumpHandler>();
        playerManager = GetComponent<PlayerManager>();
        animator = GetComponent<Animator>();

        groundCheck = GetComponent<GroundCheck>();


        climbCollider = GetComponent<BoxCollider>();

        controller.enableOverlapRecovery = true;
        controller.detectCollisions = true;

        playerHeight = controller.height;
        playerRadius = controller.radius;

    }







    public void StartClimb()   //searching for climable nodes or ledges
    {

        if (Physics.Raycast(raycastForward.transform.position, transform.forward, out RaycastHit ledgeHit, climbRayLength, ledgelayer, previewClimb, 1f, Color.green, Color.red))
        {
            canClimb = true;   //if this RC is touching something then the player can climb and CANNOT JUMP

            if (jumpAction.IsPressed())  //if jump is pressed annd player is grounded AND raycast positive
            {
                ClimbBegins(ledgeHit);
            }
        }
        else
        {
            canClimb = false;
        }
    }



    private void ClimbBegins(RaycastHit ledgeHit)
    {
        animator.SetBool("startClimb", true);  //start animation of player

        lastValidHangPoint = ledgeHit.point;  //creates the initial climb point

        surfaceNormal = -ledgeHit.normal;
        previousNormal = surfaceNormal;


        playerManager.ChangeState(PlayerManager.PlayerState.FreeClimb);

        PlayerClimbCollisions(true);
    }



    public void FreeClimb()
    {
        Vector2 climbInput = moveAction.ReadValue<Vector2>();
        climbDirection = new Vector3(climbInput.x, climbInput.y, 0);

        Vector3 climbDirLocalized = transform.InverseTransformDirection(climbDirection); //changes input to local position

        if (Physics.Raycast(raycastForward.transform.position, transform.forward, out RaycastHit ledgeHit, 2f, ledgelayer, previewClimb, 1f, Color.green, Color.red))

        {
            corneringLeft = false;
            corneringRight = false;

            lastValidHangPoint = transform.position;  //saving a return point for when player goes out of bounds

            surfaceNormal = -ledgeHit.normal;

            if (surfaceNormal != previousNormal) //did the angle change?
            {
                StartCoroutine(ClimbRotation(ledgeHit));  //rotates player along surface normals
                previousNormal = surfaceNormal;  //records new normal
            }
            else
            {
                transform.forward = -ledgeHit.normal;
            }

            //animation info
            float climbMag = climbDirLocalized.sqrMagnitude;
            animator.SetFloat("climbX", climbMag);

            climbDirLocalized.z = climbDirLocalized.z * -1; //makes sure player doesn't float away while climbing

            controller.Move(climbDirLocalized * climbSpeed * Time.deltaTime); //player moves


            if (ledgeHit.distance > .40f)  //iof player drifts too far away, they come back to wall
            {
                transform.position += surfaceNormal * 2 * Time.deltaTime;
            }

            else if (ledgeHit.distance < .35f)  //if they start to clip into wall, they go back to wall
            {
                transform.position -= surfaceNormal * 2 * Time.deltaTime;
            }


            if (climbDirection.y > 0.5f && !Physics.Raycast(raycastAbove.transform.position, transform.forward, out RaycastHit topDetect, 1f, everythingLayer, previewClimb, 1f, Color.green, Color.red))
            {
                //if the player is pressing up and reaches an area where there is no more wall to climb
                climbingUp = true;

                if (Physics.Raycast(raycastAboveAhead.transform.position, Vector3.down, out RaycastHit ledgeDetected, 1.5f, everythingLayer, previewClimb, 1f, Color.green, Color.red))

                //and there is space in that empty area for the player to stand

                {
                    animator.SetBool("finishClimb", true);
                    animator.SetBool("startClimb", false);

                    transform.DOMove(ledgeDetected.point, 1f).SetEase(Ease.InOutQuad);

                    StartCoroutine(ClimbCancel());

                    transform.position = ledgeDetected.point;
                }

                transform.position = lastValidHangPoint;
            }
        }

        else
        {

            if (climbDirection.x < 0 && corneringLeft == false)
            {
                corneringLeft = true;
            }

            else if (climbDirection.x > 0 && corneringRight == false)
            {
                corneringRight = true;
            }



            else
            {
                if (corneringLeft == false && corneringRight == false && climbingUp == false)
                {

                    StopAllCoroutines();
                    Debug.Log($"Attempting to relocate player to {lastValidHangPoint}");

                    transform.position = lastValidHangPoint;
                }
            }
        }

        if (corneringLeft)
        {
            StartCoroutine(ClimbCorner(transform.position, transform.rotation.eulerAngles, -transform.right, 90f));
        }

        else if (corneringRight)
        {
            StartCoroutine(ClimbCorner(transform.position, transform.rotation.eulerAngles, transform.right, -90f));
        }
    }


    public IEnumerator ClimbRotation(RaycastHit ledgeHit)
    {

        float rotationSpeed = 2;

        Quaternion targetRotation = Quaternion.Euler(-ledgeHit.normal);   // find the camera's Y rotation

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        yield return null;

    }



    public IEnumerator ClimbCorner(Vector3 lastPosition, Vector3 lastRotation, Vector3 direction, float angle)
    {

        Debug.Log("Cornering!");
        climbCollider.enabled = false;

        transform.position = transform.position + direction * Time.deltaTime * cornerDirectionAmount;

        Vector3 newPosition = transform.position;

        yield return new WaitForSeconds(.25f);
        transform.position = transform.position + transform.forward * Time.deltaTime * cornerFowardAmount;
        yield return new WaitForSeconds(.25f);

        transform.DOLocalRotate(lastRotation + new Vector3(0, angle, 0), .25f);

        yield return new WaitForSeconds(.5f);

        climbCollider.enabled = true;


        yield return null;
    }




    public IEnumerator ClimbCancel()  //input from crouch button
    {
        canClimb = true;

        animator.SetBool("isDropping", true);

        PlayerClimbCollisions(false);

        animator.SetBool("startClimb", false);
        animator.SetBool("finishClimb", false);
        //climbing.weight = 0;

        //player drops from ledge

        yield return null;



        yield return new WaitForSeconds(1);
        playerManager.ChangeState(PlayerManager.PlayerState.NoInput);

        animator.SetBool("isClimbing", false);
        animator.SetBool("isDropping", false);

        canClimb = false;  //prevent jump f
        StopAllCoroutines();
        playerManager.ChangeState(PlayerManager.PlayerState.BasicMovement);

    }



    public Vector3 OrientPlayer()
    {
        //ORIENT PLAYER
        if (Physics.Raycast(raycastForward.transform.position, transform.forward, out RaycastHit forwardHit, 2f, ledgelayer))
        {
            Debug.Log(forwardHit.normal);

            return forwardHit.normal;
        }
        else
        {
            return transform.forward;
        }
    }


    private void PlayerClimbCollisions(bool isClimb)
    {
        controller.detectCollisions = !isClimb;
        climbCollider.enabled = isClimb;

        if (isClimb)
        {
            controller.height = 0;
            controller.radius = 0;
        }
        else
        {
            controller.height = playerHeight;
            controller.radius = playerRadius;
        }





    }

}
