using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public enum ItemSpawnerType
{
    keyCommon,
    keyRare,
    largeObject
}

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] ItemSpawnerType itemSpawnerType;

    ItemType itemTypeToSpawn;
    bool hasParent = false;
    Vector3 originalParentOffeset;

    void Awake()
    {
        ItemsSpawnerManager.instance.AddItemSpawner(this);

        if (transform.parent != null)
        {
            hasParent = true;
            originalParentOffeset = transform.position - transform.parent.gameObject.transform.position;
        }
    }

    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
    }

    void SpawnBase()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        ItemData itemData = ItemsDataManager.instance.GetItemData(itemTypeToSpawn);

        GameObject collectableItem = Instantiate(itemData.collectableItemPrefab);
        collectableItem.transform.position = transform.position;
        collectableItem.transform.rotation = transform.rotation;

        if (hasParent)
        {
            collectableItem.GetComponent<NetworkTransformChild>().SetTarget(transform.parent, originalParentOffeset);
        }

        collectableItem.GetComponent<NetworkObject>().Spawn(true);
    }

    public void Spawn(ItemType itemTypeToSpawn)
    {
        this.itemTypeToSpawn = itemTypeToSpawn;
        SpawnBase();
    }

    public ItemSpawnerType GetItemSpawnerType()
    {
        return itemSpawnerType;
    }
}
