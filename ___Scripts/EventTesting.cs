using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTesting : MonoBehaviour
{

    DelegateTesting delegateTesting;



    private void OnEnable()
    {
        DelegateTesting.OnClicked += Teleport;   // Onlicked (the Event being raised in Event Manager) is being subscribed to by Teleport

    }

    private void OnDisable()
    {
        DelegateTesting.OnClicked -= Teleport;  //unsubscriing

    }


    void Teleport()    //the event being Subscribed to, and triggers when raised
    {
        Vector3 pos = transform.position;
        pos.y = Random.Range(.3f, 1.0f);
        transform.position = pos;
    }

}

