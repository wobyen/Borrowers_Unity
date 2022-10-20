using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteParent : MonoBehaviour
{

    [SerializeField] GameObject remoteParent;


    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {

        transform.position = remoteParent.transform.position;


    }
}
