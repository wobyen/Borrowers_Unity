using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ChickenBehavior : MonoBehaviour

{

    // public Transform target;
    Animator anim;

    public AudioSource chickenDeath;

    public int chickenHealth = 10;

    public GameObject explosion;


    public Vector3 walkPoint;

    public bool walkPointSet;
    public float walkPointRange;

    UnityEngine.AI.NavMeshAgent agent;

    public LayerMask Ground;

    public EventsManager eventsManager;


    // Angular speed in radians per sec.
    public float speed = 1.0f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        // player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();


    }

    private void Start()
    {
        eventsManager = GameObject.Find("EventsManager").GetComponent<EventsManager>();
        eventsManager.numberOfChickens++;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            chickenDeath.Play();

            chickenHealth -= 10;
            Debug.Log("Chicken lost health!");

            if (chickenHealth <= 0)
            {

                eventsManager.numberOfChickens--;
                eventsManager.chickensKilled++;


                Instantiate(explosion, transform.position, transform.rotation);
                gameObject.SetActive(false);

            }
        }
    }



    void Update()
    {
        if (!walkPointSet) SearchWalkPoint();

        else if (walkPointSet)
            agent.SetDestination(walkPoint);


        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        anim.SetBool("isWalking", true);

        if (distanceToWalkPoint.magnitude <= 1f)
        {
            walkPointSet = false;
            anim.SetBool("isWalking", false);
        }

    }



    private void SearchWalkPoint()
    {
        Debug.Log("set walkpoint");

        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        float randomX = Random.Range(-walkPointRange, walkPointRange);

        Debug.Log(randomZ);
        Debug.Log(randomX);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, Ground))
        {
            walkPointSet = true;
            Debug.Log("Walkpoint set to " + walkPoint);


        }
    }

    private void OnDrawGizmos()
    {
        {

            Gizmos.color = Color.red;
            Gizmos.DrawCube(walkPoint, new Vector3(.5f, .5f, .5f));


        }
    }
}

