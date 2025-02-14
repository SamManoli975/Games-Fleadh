using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableMaster : MonoBehaviour
{
    public List<GameObject> managedInteractables = new List<GameObject>();
    public List<Interactable> interactables = new List<Interactable>();

    public int GetInteractableNum(Interactable interactable)
    {
        for (int i = 0; i < interactables.Count; i++)
        {
            if (interactables[i] == interactable)
                return i;
        }

        return -1;
    }

    public Interactable GetInteractableByNum(int num)
    {
        if (num < 0 || num > interactables.Count)
            return null;

        return interactables[num];
    }
}
