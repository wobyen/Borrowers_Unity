using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;
using RotaryHeart.Lib.PhysicsExtension;

public class CameraFadeObjectsMode : MonoBehaviour
{


    public GameObject player;
    public Material shader;

    public static int PlayerPos = Shader.PropertyToID("_PlayerPosition");


    public float cameraRayDistance;

    // Update is called once per frame
    void Update()
    {


        var playerLocation = player.transform.position;

        shader.SetVector(PlayerPos, playerLocation);




    }









    public void ViewBlocked()
    {
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit, cameraRayDistance, PreviewCondition.Both, 1f, Color.green, Color.red))
        {
            if (!hit.collider.gameObject.CompareTag("Player"))
            {
                hit.collider.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }

        }



    }



}
