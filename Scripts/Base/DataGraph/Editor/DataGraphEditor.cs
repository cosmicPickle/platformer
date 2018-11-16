using System;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(DataGraph))]
public class DataGraphEditor : Editor {

    protected string openNodeEditorText = "Open Node Editor";
    protected string closeNodeEditorText = "Close Node Editor";
    protected string assetPath;

    protected DataGraph dataGraph;
    protected NodeBasedEditor nodeBasedEditor;

    protected const float saveAssetsInterval = 15f;
    protected double lastSave;

    private void OnEnable()
    {
        dataGraph = (DataGraph)target;
        assetPath = AssetDatabase.GetAssetPath(dataGraph);
        
        if(assetPath.Length > 0)
        {
            assetPath = Path.GetDirectoryName(assetPath);
        }

        OnNodeEditorDataChange();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Size", dataGraph.Size.ToString());

        if (GUILayout.Button(openNodeEditorText))
        {
            if (nodeBasedEditor == null)
            {
                nodeBasedEditor = BindNodeEditorWindow();
                nodeBasedEditor.onDataChange -= OnNodeEditorDataChange;
                nodeBasedEditor.onDataChange += OnNodeEditorDataChange;
            }

            nodeBasedEditor.Show();
            nodeBasedEditor.ClearData();
            nodeBasedEditor.SetData(ref dataGraph, OnCreateDataNode, OnRemoveDataNode, OnCreateDataEdge, OnRemoveDataEdge);
            nodeBasedEditor.Repaint();
        }

        if (GUILayout.Button(closeNodeEditorText))
        {
            if (nodeBasedEditor == null)
            {
                nodeBasedEditor = BindNodeEditorWindow();
                nodeBasedEditor.onDataChange -= OnNodeEditorDataChange;
                nodeBasedEditor.onDataChange += OnNodeEditorDataChange;
            }

            nodeBasedEditor.ClearData();
            nodeBasedEditor.Repaint();

            nodeBasedEditor = null;
        }

        OnNodeEditorDataChange();
    }

    protected virtual NodeBasedEditor BindNodeEditorWindow()
    {
        return EditorWindow.GetWindow<NodeBasedEditor>();  
    }

    protected virtual void OnNodeEditorDataChange() {
        EditorUtility.SetDirty(target);

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
    }

    protected virtual DataGraphNode OnCreateDataNode()
    {
        DataGraphNode newNode = CreateInstance<DataGraphNode>();

        AssetDatabase.AddObjectToAsset(newNode, assetPath + Path.DirectorySeparatorChar + dataGraph.name + ".asset");

        newNode.uiSettings = null;

        dataGraph.AddNode(newNode);
        OnNodeEditorDataChange();

        Debug.Log(newNode.uiSettings);

        return newNode;
    }

    protected virtual void OnRemoveDataNode(DataGraphNode node)
    {
        dataGraph.RemoveNode(node);
        DestroyImmediate(node, true);

        OnNodeEditorDataChange();
    }

    protected virtual void OnCreateDataEdge(Connection connection)
    {
        dataGraph.AddEdge(connection.outPoint.node.dataNode, connection.inPoint.node.dataNode);
        OnNodeEditorDataChange();
    }

    protected virtual void OnRemoveDataEdge(Connection connection)
    {
        dataGraph.RemoveEdge(connection.outPoint.node.dataNode, connection.inPoint.node.dataNode);
        OnNodeEditorDataChange();
    }
}
