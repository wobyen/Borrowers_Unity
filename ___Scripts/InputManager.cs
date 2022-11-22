using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    PlayerControls playerControls;


    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction sprintAction;
    public InputAction crouchAction;
    public InputAction pauseAction;
    public InputAction useAction;



    private void OnEnable()
    {
        playerControls = new PlayerControls();


        moveAction = playerControls.Player.Move;
        moveAction.Enable();

        jumpAction = playerControls.Player.Jump;
        jumpAction.Enable();

        sprintAction = playerControls.Player.Sprint;
        sprintAction.Enable();

        crouchAction = playerControls.Player.Crouch;
        crouchAction.Enable();

        useAction = playerControls.Player.Use;
        useAction.Enable();

        pauseAction = playerControls.Player.Pause;
        pauseAction.Enable();
    }



    private void OnDisable()
    {
        moveAction.Disable();

        jumpAction.Disable();

        sprintAction.Disable();

        crouchAction.Disable();

        useAction.Disable();

        pauseAction.Disable();


    }

}
