using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CollectableItem))]
public class CollectableItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CollectableItem script = (CollectableItem)target;

        if (GUILayout.Button("Setup Components"))
        {
            Component[] modifiedComponents = script.SetupComponents();
            foreach (Component c in modifiedComponents)
            {
                EditorUtility.SetDirty(c);
            }
        }
    }
}
