using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public enum ItemSpawnerType
{
    keyCommon,
    keyRare,
    largeObject,
    keyGate
}

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] ItemSpawnerType itemSpawnerType;
    [SerializeField] Transform overrideParent;

    ItemType itemTypeToSpawn;
    bool hasParent = false;
    Vector3 originalParentOffeset;

    Transform parent;

    void Awake()
    {
        ItemsSpawnerManager.instance.AddItemSpawner(this);

        parent = overrideParent;
        if (parent == null)
        {
            parent = transform.parent;
        }

        if (parent != null)
        {
            hasParent = true;
            originalParentOffeset = transform.position - parent.position;
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
            collectableItem.GetComponent<NetworkTransformChild>().SetTarget(parent, originalParentOffeset);
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
