using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;

public class PushObjects : MonoBehaviour
{

    private float forceMag;

    Animator animator;

    public bool pushing;

    InputAction useAction;

    PlayerControls playerControls;
    PlayerMovement playerMovement;
    CharacterController controller;

    PlayerManager playerManager;
    Vector3 forceDirection;
    public Vector3 objectOrientation;
    InputAction moveAction;

    public Vector2 moveInput;

    public float pushSpeed = 1;

    [SerializeField] PreviewCondition previewConditionPush;

    [SerializeField] GameObject forwardRaycaster;




    private void OnEnable()
    {
        useAction = playerControls.Player.Use;
        useAction.Enable();



        moveAction = playerControls.Player.Move;
        moveAction.Enable();

    }

    private void OnDisable()
    {
        useAction.Disable();

        moveAction.Disable();

    }
    private void Awake()
    {
        playerControls = new PlayerControls();

        playerManager = GetComponent<PlayerManager>();

        playerMovement = GetComponent<PlayerMovement>();

        controller = GetComponent<CharacterController>();

        animator = GetComponent<Animator>();

    }



    private void Update()
    {
        //Physics.Raycast(forwardRaycaster.transform.position, transform.forward, out RaycastHit objectHit, Mathf.Infinity, previewConditionPush, 2f, Color.blue, Color.blue);
        //raycast tests

    }



    public void PushingObject(Rigidbody pushableRB)

    {
        animator.Play("Push");

        Debug.Log("Trying to push");
        Physics.Raycast(forwardRaycaster.transform.position, transform.forward, out RaycastHit objectHit, 1f, previewConditionPush, 2f, Color.green, Color.red);

        Debug.Log($"This is the normal: {objectHit.normal}");

        objectOrientation = objectHit.normal;


        moveInput = moveAction.ReadValue<Vector2>();  //input from player move 

        controller.Move(-objectOrientation * moveInput.sqrMagnitude * Time.deltaTime);  // the input moves the player

        pushableRB.gameObject.transform.Translate(objectOrientation * Time.deltaTime);  //box moves with player



        if (useAction.triggered)  //end the push
        {
            animator.Play("Walking");

            playerManager.ChangeState(PlayerManager.PlayerState.BasicMovement);
        }



        //IF NEED TO SWITCH TO PHYSICS ENGINE//
        //------------
        //forceDirection = pushableRB.transform.position - transform.position;
        //forceDirection.y = 0;
        // forceDirection.Normalize();
        // forceMag = 100;

        //pushableRB.AddForceAtPosition(forceMag * -objectOrientation, transform.position, ForceMode.Impulse);



    }



}
