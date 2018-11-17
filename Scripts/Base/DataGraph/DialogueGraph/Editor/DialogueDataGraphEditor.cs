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

        int result = dataGraph.AddNode(newNode);

        if (result >= 0)
        {
            OnNodeEditorDataChange();
            return newNode;
        }

        return null;
    }

    protected override void OnNodeEditorDataChange()
    {
        DialogueDataGraph dialogueGraph = (DialogueDataGraph)dataGraph;

        dialogueGraph.nodes.ForEach(node =>
        {
            DialogueDataNode dialogueDataNode = (DialogueDataNode)node;

            if(dialogueDataNode.type == DialogueDataNode.Type.StartDialogue && dialogueGraph.startDialogue == null)
            {
                dialogueGraph.startDialogue = dialogueDataNode;
            }
        });

        base.OnNodeEditorDataChange();
    }
}
