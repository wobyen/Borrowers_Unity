using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperUI : MonoBehaviour
{

    TextMesh textMesh;
    public GameObject debugObject;


    void Start()
    {

        textMesh = GetComponent<TextMesh>();

    }

    // Update is called once per frame
    void Update()
    {

        transform.position = debugObject.transform.position + Vector3.up * 2f;

        textMesh.transform.rotation = Camera.main.transform.rotation;
        textMesh.text = $"{transform.position.ToString()}";
    }
}
