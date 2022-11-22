using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryHandler : InputManager
{
    public InventoryObject inventory;

    public bool resetInventory;

    public GUI_TextObject textObject;

    public NotificationHandler notifications;


    bool nearItem = false;

    Collider nearbyItem;


    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Item near");
        nearItem = true;
        nearbyItem = other;
    }


    private void OnTriggerExit(Collider other)
    {
        Debug.Log("No item");
        nearItem = false;

        nearbyItem = null;
    }

    private void Update()
    {


        if (useAction.IsPressed() && nearItem && nearbyItem != null)
        {

            PickUpItem(nearbyItem);
        }

    }


    public void PickUpItem(Collider other)
    {
        {
            var item = other.GetComponent<ItemBehavior>();  //get the ItemBehavior component 
            if (item)
            {
                Debug.Log("Item deteected -- adding to inventory");
                inventory.AddItem(item.item, 1);   //if there's one there, add it to inventory using AddItem function
                                                   // textObject.textDisplay = $"You picked up a {item.name}";

                //StartCoroutine(notifications.TextFadeInOut());

                Destroy(other.gameObject);
                nearbyItem = null;
            }
        }
    }


    private void OnApplicationQuit()
    {
        if (resetInventory)
            inventory.Container.Clear();

    }


}



