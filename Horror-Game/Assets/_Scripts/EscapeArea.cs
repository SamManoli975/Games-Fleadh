using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeArea : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Survivor survivor = other.GetComponent<Survivor>();
        if (survivor != null)
        {
            GameManager.instance.EndGame(PlayerRole.survivor);
        }
    }
}
