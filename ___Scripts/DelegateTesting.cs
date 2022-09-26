using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DelegateTesting : MonoBehaviour
{



    public delegate void ClickAction();  //declaring a new delegate type 
                                         //referencing a VOID method 
                                         //with no args called ClickAction


    public static event ClickAction OnClicked;   //creating an event of 
                                                 //the delegate type 
                                                 //"Click Action".  
                                                 //The event is OnClicked.  


    private void Update()
    {

        if (Input.GetKeyDown("space"))
        {


            OnClicked?.Invoke();

        }



    }
}