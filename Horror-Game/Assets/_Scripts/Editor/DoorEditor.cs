using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Door script = (Door)target;

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
