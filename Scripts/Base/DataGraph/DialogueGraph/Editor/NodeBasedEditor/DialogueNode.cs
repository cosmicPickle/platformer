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

    public override bool IsConnectionAllowed(Node other, bool isNewConnection = false)
    {
        DialogueDataNode dNode = (DialogueDataNode)dataNode; 
        DialogueDataNode otherDNode = (DialogueDataNode)other.dataNode;
        DialogueDataGraph graph = (DialogueDataGraph)currentEditor.GetData();
        int connectionCount = graph.GetNodeConnections(dNode).Count;

        if(otherDNode.type == DialogueDataNode.Type.StartDialogue)
        {
            return false;
        }


        switch (dNode.type)
        {
            case DialogueDataNode.Type.EndDialogue:
                return false;
            case DialogueDataNode.Type.StartDialogue:
                if(otherDNode.type == DialogueDataNode.Type.Answer)
                {
                    return false;
                }
                if (connectionCount > (isNewConnection ? 0 : 1))
                {
                    return false;
                }
                break;
            case DialogueDataNode.Type.Text:
                if (connectionCount > (isNewConnection ? 0 : 1))
                {
                    return false;
                }
                break;
            case DialogueDataNode.Type.Question:
                if (otherDNode.type != DialogueDataNode.Type.Answer || connectionCount > (isNewConnection ? 3 : 4))
                    return false;
                break;
            case DialogueDataNode.Type.Answer:
                if (otherDNode.type == DialogueDataNode.Type.Answer)
                    return false;
                if (connectionCount > (isNewConnection ? 0 : 1))
                    return false;
                break;
            case DialogueDataNode.Type.Condition:
                if (connectionCount > (isNewConnection ? 1 : 2))
                    return false;
                break;
        }

        return true;
    }

    public override bool IsNodeAllowed(bool isNewNode = false)
    {
        DialogueDataNode nd = (DialogueDataNode)dataNode;
        
        if (nd.type == DialogueDataNode.Type.StartDialogue)
        {
            DialogueDataGraph graph = (DialogueDataGraph)currentEditor.GetData();

            if (graph.startDialogue != null && graph.startDialogue != nd)
            {
                return false;
            }
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
