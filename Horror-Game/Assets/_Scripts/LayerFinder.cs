using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerFinder : MonoBehaviour
{
    [SerializeField] int targetLayer = 0;

    [ContextMenu("Find On Layer")]
    public void FindOnLayer()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>(); // Finds all active objects
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == targetLayer)
            {
                Debug.Log("Object found on layer: " + obj.name);
            }
        }
    }
}
