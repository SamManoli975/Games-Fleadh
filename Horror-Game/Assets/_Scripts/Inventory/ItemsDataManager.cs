using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsDataManager : MonoBehaviour
{
    public static ItemsDataManager instance;

    [SerializeField] ItemsDataSO itemsDataSO;
    Dictionary<ItemType, ItemData> itemTypeMap = new Dictionary<ItemType, ItemData>();

    void Awake()
    {
        instance = this;

        FillItemTypeMap();
    }

    public ItemData GetItemData(ItemType itemType)
    {
        if (!itemTypeMap.ContainsKey(itemType))
        {
            Debug.LogError("No item data defined for item of type '" + itemType + "'");
            return null;
        }

        return itemTypeMap[itemType];
    }

    void FillItemTypeMap()
    {
        for (int i = 0; i < itemsDataSO.items.Count; i++)
        {
            itemTypeMap.Add(itemsDataSO.items[i].itemType, itemsDataSO.items[i]);
        }
    }
}
