using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataGraph))]
public class DataGraphEditor : Editor {

    protected string openNodeEditorText = "Open Node Editor";
    protected string closeNodeEditorText = "Close Node Editor";
    protected string saveChangesText = "Save Changes";

    protected DataGraph dataGraph;
    protected NodeBasedEditor nodeBasedEditor;

    protected const float saveAssetsInterval = 15f;
    protected double lastSave;
    protected bool isDirty;

    private void OnEnable()
    {
        dataGraph = (DataGraph)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Size", dataGraph.Size.ToString());

        if (GUILayout.Button(openNodeEditorText))
        {
            if (nodeBasedEditor == null)
            {
                nodeBasedEditor = EditorWindow.GetWindow<NodeBasedEditor>();
                nodeBasedEditor.onDataChange -= OnNodeEditorDataChange;
                nodeBasedEditor.onDataChange += OnNodeEditorDataChange;
            }

            nodeBasedEditor.Show();
            nodeBasedEditor.SetData(ref dataGraph);
            nodeBasedEditor.Repaint();
        }

        if (GUILayout.Button(closeNodeEditorText))
        {
            if (nodeBasedEditor == null)
            {
                nodeBasedEditor = EditorWindow.GetWindow<NodeBasedEditor>();
                nodeBasedEditor.onDataChange -= OnNodeEditorDataChange;
                nodeBasedEditor.onDataChange += OnNodeEditorDataChange;
            }

            nodeBasedEditor.ClearData();
            nodeBasedEditor.Repaint();

            nodeBasedEditor = null;
        }

        EditorGUI.BeginDisabledGroup(!isDirty);
        if (GUILayout.Button(saveChangesText))
        {
            Save();
        }
        EditorGUI.EndDisabledGroup();
    }

    protected virtual void OnNodeEditorDataChange() {
        EditorUtility.SetDirty(target);
        isDirty = true;

        if (lastSave + saveAssetsInterval <= EditorApplication.timeSinceStartup)
        {
            Save();
        }
    }

    protected void Save()
    {
        lastSave = EditorApplication.timeSinceStartup;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        isDirty = false;
    }
}
