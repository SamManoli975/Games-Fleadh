using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Drawer))]
public class DrawerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Drawer script = (Drawer)target;

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