using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryHandler : MonoBehaviour
{
    public InventoryObject inventory;

    public bool resetInventory;

    public GUI_TextObject textObject;

    public NotificationHandler notifications;

    PlayerControls playerControls;
    InputAction useAction;

    private void Awake()
    {
        playerControls = new PlayerControls();

    }

    private void OnEnable()
    {
        useAction = playerControls.Player.Use;

        useAction.Enable();

    }


    private void OnDisable()
    {
        useAction.Disable();


    }




    public void OnTriggerEnter(Collider other)
    {

        if (useAction.IsPressed())
        {
            var item = other.GetComponent<ItemBehavior>();
            if (item)
            {
                inventory.AddItem(item.item, 1);
                textObject.textDisplay = $"You picked up a {item.name}";


                StartCoroutine(notifications.TextFadeInOut());

                Destroy(other.gameObject);
            }
        }
    }


    private void OnApplicationQuit()
    {
        if (resetInventory)
            inventory.Container.Clear();

    }


}



