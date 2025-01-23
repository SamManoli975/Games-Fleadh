using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    flashlight,
    keyA,
    keyB
}

[System.Serializable]
public class ItemData
{
    public ItemType itemType;
    public string name;
    public Sprite image;
    public bool isStackable;
}
