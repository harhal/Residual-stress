#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class ActorLayoutEditorWindow : EditorWindow
{
    private ActorsLayoutData _layout;
    private Transform _root;
    private int _selectedIndex = -1;
    private bool _editRotation = false;

    public static void Open(ActorsLayoutData layout)
    {
        var wnd = GetWindow<ActorLayoutEditorWindow>("Actor Layout Editor");
        wnd._layout = layout;
        wnd.Show();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        Selection.selectionChanged += Repaint;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        Selection.selectionChanged -= Repaint;
    }

    private void OnGUI()
    {
        if (_layout == null)
        {
            EditorGUILayout.HelpBox("Assign an ActorsLayoutData asset to edit.", MessageType.Info);
            return;
        }

        EditorGUILayout.ObjectField("Layout", _layout, typeof(ActorsLayoutData), false);

        _root = (Transform)EditorGUILayout.ObjectField("Root (anchor)", _root, typeof(Transform), true);
        _editRotation = EditorGUILayout.ToggleLeft("Edit rotation", _editRotation);

        EditorGUILayout.Space();

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Add Slot"))
            {
                AddSlot();
            }

            using (new EditorGUI.DisabledScope(_selectedIndex < 0 || _selectedIndex >= _layout.entries.Count))
            {
                if (GUILayout.Button("Remove Selected"))
                {
                    RemoveSelected();
                }
            }
        }

        EditorGUILayout.Space();

        DrawSlotsList();
    }

    private void DrawSlotsList()
    {
        var entries = _layout.entries;

        for (int i = 0; i < entries.Count; i++)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                bool isSel = i == _selectedIndex;

                if (GUILayout.Toggle(isSel, "", GUILayout.Width(18)))
                {
                    _selectedIndex = i;
                }

                entries[i] = EditEntryInline(entries[i]);
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(_layout);
        }
    }

    private ActorsLayoutData.Entry EditEntryInline(ActorsLayoutData.Entry e)
    {
        e.id = EditorGUILayout.TextField(e.id);

        if (GUILayout.Button("Focus", GUILayout.Width(60)))
        {
            FocusOnEntry(e);
        }

        return e;
    }

    private void FocusOnEntry(ActorsLayoutData.Entry e)
    {
        var worldPos = LocalToWorld(e.localPosition);
        SceneView.lastActiveSceneView.LookAt(worldPos);
    }

    private void AddSlot()
    {
        Undo.RecordObject(_layout, "Add Layout Slot");

        var e = new ActorsLayoutData.Entry
        {
            id = $"Slot_{_layout.entries.Count + 1}",
            localPosition = Vector3.zero,
            localEuler = Vector3.zero
        };

        _layout.entries.Add(e);
        _selectedIndex = _layout.entries.Count - 1;

        EditorUtility.SetDirty(_layout);
    }

    private void RemoveSelected()
    {
        Undo.RecordObject(_layout, "Remove Layout Slot");

        _layout.entries.RemoveAt(_selectedIndex);
        _selectedIndex = Mathf.Clamp(_selectedIndex - 1, -1, _layout.entries.Count - 1);

        EditorUtility.SetDirty(_layout);
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (_layout == null)
        {
            return;
        }

        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10, 10, 280, 90), "Layout Editor", GUI.skin.window);
        GUILayout.Label(_layout.name);
        GUILayout.Label(_root ? $"Root: {_root.name}" : "Root: (none, world origin)");
        GUILayout.Label(_selectedIndex >= 0 ? $"Selected: {_selectedIndex}" : "Selected: none");
        GUILayout.EndArea();
        Handles.EndGUI();

        DrawAndEditHandles();
    }

    private void DrawAndEditHandles()
    {
        var entries = _layout.entries;

        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];

            Vector3 worldPos = LocalToWorld(e.localPosition);
            Quaternion worldRot = LocalToWorldRot(Quaternion.Euler(e.localEuler));

            float size = HandleUtility.GetHandleSize(worldPos) * 0.12f;

            Handles.Label(worldPos + Vector3.up * size, string.IsNullOrWhiteSpace(e.id) ? $"[{i}]" : e.id);

            // Клик по точке = выбор
            if (Handles.Button(worldPos, Quaternion.identity, size, size, Handles.SphereHandleCap))
            {
                _selectedIndex = i;
                Repaint();
            }

            if (i != _selectedIndex)
            {
                continue;
            }

            EditorGUI.BeginChangeCheck();

            Vector3 newWorldPos = Handles.PositionHandle(worldPos, worldRot);
            Quaternion newWorldRot = worldRot;

            if (_editRotation)
            {
                newWorldRot = Handles.RotationHandle(worldRot, worldPos);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_layout, "Move Layout Slot");

                e.localPosition = WorldToLocal(newWorldPos);

                if (_editRotation)
                {
                    var localRot = WorldToLocalRot(newWorldRot);
                    e.localEuler = localRot.eulerAngles;
                }

                entries[i] = e;

                EditorUtility.SetDirty(_layout);
            }
        }
    }

    private Vector3 LocalToWorld(Vector3 localPos)
    {
        if (_root == null)
        {
            return localPos;
        }

        return _root.TransformPoint(localPos);
    }

    private Vector3 WorldToLocal(Vector3 worldPos)
    {
        if (_root == null)
        {
            return worldPos;
        }

        return _root.InverseTransformPoint(worldPos);
    }

    private Quaternion LocalToWorldRot(Quaternion localRot)
    {
        if (_root == null)
        {
            return localRot;
        }

        return _root.rotation * localRot;
    }

    private Quaternion WorldToLocalRot(Quaternion worldRot)
    {
        if (_root == null)
        {
            return worldRot;
        }

        return Quaternion.Inverse(_root.rotation) * worldRot;
    }
}
#endif