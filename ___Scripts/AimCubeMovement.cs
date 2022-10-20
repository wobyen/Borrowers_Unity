using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class AimCubeMovement : MonoBehaviour
{


    public CinemachineFreeLook activeCam;
    public GameObject player;
    public float aimSpeed = 5;
    public float yOffset = 0;
    public float yModifier = 1;
    public float zVal = 3;

    [SerializeField] float yOff;

    [SerializeField] float rotationSpeed = 10;

    PlayerControls playerControls;

    InputAction lookAction;
    InputAction jumpAction;
    InputAction sprintAction;
    InputAction crouchAction;
    InputAction aimAction;



    private void Awake()
    {
        playerControls = new PlayerControls();


    }

    private void OnEnable()
    {
        lookAction = playerControls.Player.Look;
        lookAction.Enable();

        //transform.localPosition = new Vector3(0, 1, -7);

    }

    private void OnDisable()
    {
        lookAction.Disable();

    }


    private void Update()

    {

        // transform.position = new Vector3(transform.position.x, activeCam.transform.position.y * yOff, transform.position.z);

        Vector3 aimInput = lookAction.ReadValue<Vector2>();
        Vector3 aimNorm = aimInput.normalized;  //input for hang movement


        Vector3 aimVector = new Vector3(0, 1 - activeCam.transform.position.y - activeCam.m_YAxis.Value + yOffset * yModifier, zVal); //find next ledge

        transform.localPosition = aimVector;

        Debug.Log(activeCam.m_YAxis.Value);



    }





}
