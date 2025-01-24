using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsInfo", menuName = "ScriptableObjects/ItemsDataSO", order = 1)]
public class ItemsDataSO : ScriptableObject
{
    public List<ItemData> items;
}