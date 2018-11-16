using System;
using UnityEditor;
using UnityEngine;

public class DialogueNode : Node
{
    public DialogueNode(
        Vector2 position,
        Action<ConnectionPoint> OnClickInPoint,
        Action<ConnectionPoint> OnClickOutPoint,
        Action<Node> OnClickRemoveNode,
        DataGraphNode newDataNode,
        DialogueNodeBasedEditor editor
    ) : base(
        position,
        OnClickInPoint,
        OnClickOutPoint,
        OnClickRemoveNode,
        newDataNode,
        editor
    )
    {

    }

    public override bool IsConnectionAllowed(Node other)
    {
        DialogueDataNode dNode = (DialogueDataNode)dataNode; 
        DialogueDataNode otherDNode = (DialogueDataNode)other.dataNode;
        DialogueDataGraph graph = (DialogueDataGraph)currentEditor.GetData();
        int connectionCount = graph.GetNodeConnections(dNode).Count;

        switch (dNode.type)
        {
            case DialogueDataNode.Type.Text:
                if (connectionCount > 0)
                {
                    return false;
                }
                break;
            case DialogueDataNode.Type.Question:
                if (otherDNode.type != DialogueDataNode.Type.Answer)
                    return false;
                break;
            case DialogueDataNode.Type.Answer:
                if (otherDNode.type == DialogueDataNode.Type.Answer)
                    return false;
                if (connectionCount > 0)
                    return false;
                break;
            case DialogueDataNode.Type.Condition:
                if (connectionCount > 1)
                    return false;
                break;
        }

        return true;
    }

    protected override void SetStyles()
    {
        base.SetStyles();

        width = 300;
        height = 115;
    }
}
