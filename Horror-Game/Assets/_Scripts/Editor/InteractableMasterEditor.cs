using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractableMaster))]
public class InteractableMasterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        InteractableMaster script = (InteractableMaster)target;

        if (GUILayout.Button("Setup All"))
        {
            script.interactables.Clear();
            foreach (GameObject managedInteractableObj in script.managedInteractables)
            {
                IManagedInteractable managedInteractable = managedInteractableObj.GetComponent<IManagedInteractable>()
                ;
                SetupInteractableMasterRes res = managedInteractable.SetupInteractableMaster(script);
                script.interactables.AddRange(res.interactables);
                foreach (Component c in res.modifiedComponents)
                {
                    EditorUtility.SetDirty(c);
                }
            }
        }
    }
}