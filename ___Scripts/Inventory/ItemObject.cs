using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


public enum ItemType
{
    Food,
    Tool,
    Scrap,
    Decor,
    Default,
    Wearable
}

public abstract class ItemObject : ScriptableObject
{




    public GameObject itemInventoryDisplay;
    public Image itemPreview;
    public ItemType type;

    public string itemName;

    [TextArea(15, 20)]

    public string description;

    public int itemWeight;






}
