using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class SpawnableItemData
{
    public ItemType itemType;
    public ItemSpawnerType itemSpawnerType;
    public int spawnCount;
}

public class ItemsSpawnerManager : NetworkBehaviour
{
    public static ItemsSpawnerManager instance;

    [SerializeField] List<SpawnableItemData> spawnableItemDatas;

    Dictionary<ItemSpawnerType, List<ItemSpawner>> allItemSpawners = new Dictionary<ItemSpawnerType, List<ItemSpawner>>();

    void Awake()
    {
        instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
            SpawnAllItems();
    }

    void SpawnAllItems()
    {
        for (int i = 0; i < spawnableItemDatas.Count; i++)
        {
            SpawnableItemData spawnableItemData = spawnableItemDatas[i];
            ItemSpawnerType itemSpawnerType = spawnableItemData.itemSpawnerType;
            if (allItemSpawners.ContainsKey(itemSpawnerType))
            {
                List<ItemSpawner> itemSpawners = allItemSpawners[itemSpawnerType];
                List<ItemSpawner> sortedSpawners = itemSpawners.OrderBy(x => Random.Range(0f, 1f)).ConvertTo<List<ItemSpawner>>();

                for (int k = 0; k < Mathf.Min(spawnableItemData.spawnCount, sortedSpawners.Count); k++)
                {
                    sortedSpawners[k].Spawn(spawnableItemData.itemType);
                }

                List<ItemSpawner> leftSpawners = new List<ItemSpawner>();
                for (int k = spawnableItemData.spawnCount; k < sortedSpawners.Count; k++)
                {
                    leftSpawners.Add(sortedSpawners[k]);
                }
                allItemSpawners[itemSpawnerType] = leftSpawners;
            }
        }
    }

    public void AddItemSpawner(ItemSpawner itemSpawner)
    {
        ItemSpawnerType itemSpawnerType = itemSpawner.GetItemSpawnerType();
        if (!allItemSpawners.ContainsKey(itemSpawnerType))
            allItemSpawners[itemSpawnerType] = new List<ItemSpawner>();

        allItemSpawners[itemSpawnerType].Add(itemSpawner);
    }
}
