using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Behavior : MonoBehaviour
{

    Rigidbody bulletRigidbody;
    public float bulletLife = 10.0f;

    // Start is called before the first frame update

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        float speed = 50f;
        bulletRigidbody.velocity = transform.forward * speed;

        Destroy(gameObject, bulletLife); //prettys sure this will destroy it after 5 seconds.
    }

}


