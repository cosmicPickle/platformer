using System;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(DialogueDataGraph))]
public class DialogueDataGraphEditor : DataGraphEditor
{
    protected override NodeBasedEditor BindNodeEditorWindow()
    {
        return EditorWindow.GetWindow<DialogueNodeBasedEditor>();
    }

    protected override DataGraphNode OnCreateDataNode()
    {
        DataGraphNode newNode = CreateInstance<DialogueDataNode>();

        AssetDatabase.AddObjectToAsset(newNode, assetPath + Path.DirectorySeparatorChar + dataGraph.name + ".asset");

        newNode.uiSettings = null;

        dataGraph.AddNode(newNode);
        OnNodeEditorDataChange();

        Debug.Log(newNode.uiSettings);

        return newNode;
    }
}
