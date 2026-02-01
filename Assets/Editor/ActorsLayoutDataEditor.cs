#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActorsLayoutData))]
public class ActorsLayoutDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Edit Layout In Scene"))
        {
            ActorLayoutEditorWindow.Open((ActorsLayoutData)target);
        }
    }
}
#endif