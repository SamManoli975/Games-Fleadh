using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public static SpawnPoints instance;

    public Transform survivorSpawnPoint;
    public Transform monsterSpawnPoint;

    void Awake()
    {
        instance = this;
    }
}
