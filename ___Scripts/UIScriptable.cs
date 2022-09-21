using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScriptable : MonoBehaviour
{

    ClimbingHandler climbingHandler;
    public TextMeshProUGUI jumpUIText;


    // Start is called before the first frame update
    void Start()
    {
        climbingHandler = GetComponent<ClimbingHandler>();
    }

    // Update is called once per frame
    void Update()
    {

        if (climbingHandler.canClimb)
        {
            jumpUIText.enabled = true;
        }
        else
        {
            jumpUIText.enabled = false;
        }






    }
}
