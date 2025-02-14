using System.Collections.Generic;
using UnityEngine;

public class SetupInteractableMasterRes
{
    public Component[] modifiedComponents;
    public List<Interactable> interactables;

    public SetupInteractableMasterRes(Component[] modifiedComponents, List<Interactable> interactables)
    {
        this.modifiedComponents = modifiedComponents;
        this.interactables = interactables;
    }
}
