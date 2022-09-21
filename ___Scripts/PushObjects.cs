using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;

public class PushObjects : MonoBehaviour
{

    [SerializeField] private float forceMag;

    Animator animator;

    public bool pushing;
    public bool pushHolding;

    InputAction useAction;

    PlayerControls playerControls;
    PlayerMovement playerMovement;
    CharacterController controller;

    PlayerManager playerManager;
    Vector3 forceDirection;
    public Vector3 objectOrientation;
    InputAction moveAction;

    public Vector2 moveInput;

    public float pushSpeed = 3;

    [SerializeField] PreviewCondition previewConditionPush;

    [SerializeField] GameObject forwardRaycaster;

    Rigidbody objectRB;




    private void OnEnable()
    {
        useAction = playerControls.Player.Use;
        useAction.Enable();

        useAction.performed += PushHoldDown;

        useAction.canceled += ctx => PushStop();

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




    public void PushingObject()

    {
        //raycast to find a nearby pushable object
        if (Physics.Raycast(forwardRaycaster.transform.position, transform.forward, out RaycastHit objectHit, .75f, previewConditionPush, 2f, Color.green, Color.red))
        {

            if (objectHit.transform.GetComponent<Rigidbody>())
            {
                objectOrientation = objectHit.normal;
                objectRB = objectHit.transform.GetComponent<Rigidbody>();
                Debug.Log("RigidBody detected!");

            }
            else
            {
                Debug.LogError("No Rigid Body.");
            }




            if (pushHolding && playerMovement.groundedPlayer)
            {
                animator.SetBool("pushStart", true);

                playerManager.ChangeState(PlayerManager.PlayerState.PullNPush);  //changes state so that player cannot use basic motion controls
                moveInput = moveAction.ReadValue<Vector2>();  //input from player move 


                // transform.Translate(transform.forward * pushSpeed * Time.deltaTime);
                objectRB.AddForceAtPosition(forceMag * -objectOrientation, transform.position, ForceMode.Impulse);

                transform.forward = -objectOrientation;  //turns the player to face the object

                transform.SetParent(objectRB.transform);  //parents the playe to the object

                // objectRB.gameObject.transform.Translate(objectOrientation * Time.deltaTime);  //box moves with player

                forceDirection = -objectOrientation;
                forceDirection.y = 0;
                forceDirection.Normalize();

                if (moveInput.y < 0)
                    moveInput.y = 0;
                forceMag = 20 * moveInput.y;
            }
        }
    }


    public void PushHoldDown(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)   //if player HOLDS down the USE button
        {
            pushHolding = true;  //hold is true (excecutes in update)

        }
    }

    public void PushStop()
    {
        forceMag = 0;  //set to 0 so no force is applied after the push ends
        transform.SetParent(null); //remove palyer from block

        playerManager.ChangeState(PlayerManager.PlayerState.BasicMovement);
        pushHolding = false;

        animator.SetBool("pushStart", false);


    }


}
