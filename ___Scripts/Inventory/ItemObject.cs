using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public Texture2D itemPreview;
    public ItemType type;

    [TextArea(15, 20)]
    public string description;

    public int itemWeight;






}
