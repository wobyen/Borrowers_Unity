using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class disguiseObject : InputManager
{

    public GameObject player;


    Rigidbody rb;

    MeshCollider mCollider;

    public Vector3 newPosition;

    bool insideCup = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mCollider = GetComponent<MeshCollider>();
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && useAction.triggered)
        {

            rb.detectCollisions = false;
            rb.useGravity = false;
            mCollider.enabled = false;

            transform.DORotate(new Vector3(0, 0, 180), 1f);
            transform.DOMove(player.transform.position + newPosition, 1f);

            transform.SetParent(player.transform);

            insideCup = true;

            other.GetComponent<PlayerManager>().isDisguised = true;
        }

    }


    private void Update()
    {
        if (insideCup)
        {
            if (player.GetComponent<Movement>().localMoveDir == Vector3.zero)
            {
                transform.position = player.transform.position + new Vector3(0, 1.1f, 0);
            }

            else
            {
                transform.position = player.transform.position + newPosition;
            }

        }

    }

}
