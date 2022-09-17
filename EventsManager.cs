using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{

    public GameObject chickenClone;

    public int numberOfChickens;
    public int chickensKilled;

    void Start()
    {
    }


    private void Update()
    {


        if (numberOfChickens < 10)
        {
            Debug.Log("creating chickens" + numberOfChickens);

            Instantiate(chickenClone, new Vector3(transform.position.x + 2, transform.position.y, transform.position.z), transform.rotation);
        }
    }
}