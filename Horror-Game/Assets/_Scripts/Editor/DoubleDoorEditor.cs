using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoubleDoor))]
public class DoubleDoorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DoubleDoor script = (DoubleDoor)target;

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
