using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanMovement : MonoBehaviour
{

    private NavMeshAgent humanAgent;

    private Animator animator;

    public GameObject player;

    public float minDistance;

    public GameObject[] waypoints;

    public int nextWaypoint;

    bool waiting;

    Vector3 newPos;

    public bool targetDetected;

    // Start is called before the first frame update
    void Start()
    {
        humanAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        Debug.Log(waypoints.Length);


    }

    // Update is called once per frame
    void Update()
    {

        targetDetected = GetComponent<BorrowerDetection>().targetDetected;

        if (!targetDetected)
        {
            if (!waiting)
                GoToWaypoint(nextWaypoint);
        }


        if (targetDetected)
        {
            humanAgent.SetDestination(transform.position);
        }

    }


    private void GoToWaypoint(int waypoint)
    {

        animator.SetBool("humanWalking", true);

        humanAgent.SetDestination(waypoints[waypoint].transform.position);


        if (Vector3.Distance(waypoints[waypoint].transform.position, transform.position) < 5)
        {
            Debug.Log("Finished");
            animator.SetBool("humanWalking", false);
            StartCoroutine(SwitchWaypoints());
        }

    }

    private IEnumerator SwitchWaypoints()
    {

        waiting = true;
        yield return new WaitForSeconds(5);

        if (nextWaypoint == waypoints.Length - 1)
        {
            nextWaypoint = 0;
            StopAllCoroutines();
        }
        else
        {
            nextWaypoint++;
            StopAllCoroutines();

        }

        waiting = false;
        yield return null;


    }


    // private void WalkTowardsPlayer()
    // {
    //     if (Vector3.Distance(transform.position, player.transform.position) > minDistance)
    //     {
    //         animator.SetBool("humanWalking", true);


    //         newPos = player.transform.position;

    //         humanAgent.SetDestination(newPos);


    //     }



    //     if (Vector3.Distance(transform.position, newPos) < 1)
    //     {
    //         animator.SetBool("humanWalking", false);

    //     }
    // }



}