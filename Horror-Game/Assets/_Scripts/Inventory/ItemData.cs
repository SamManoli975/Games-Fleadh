using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    none,
    flashlight,
    keyCommon,
    keyRare,
    gateKey
}

[System.Serializable]
public class ItemData
{
    public ItemType itemType;
    public string name;
    public Sprite image;
    public bool isStackable;

    public GameObject collectableItemPrefab;
    public GameObject handItemPrefab;
}
