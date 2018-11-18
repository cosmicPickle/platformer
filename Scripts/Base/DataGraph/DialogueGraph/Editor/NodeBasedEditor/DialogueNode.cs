using System;
using System.Collections.Generic;
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
        List<DataGraphNode> connections = graph.GetNodeConnections(dNode);
        int connectionCount = connections.Count;

        if(otherDNode.type == DialogueDataNode.Type.StartDialogue)
        {
            return false;
        }


        switch (dNode.type)
        {
            case DialogueDataNode.Type.EndDialogue:
                return false;
            case DialogueDataNode.Type.StartDialogue:
                if (otherDNode.type == DialogueDataNode.Type.Answer
                    || otherDNode.type == DialogueDataNode.Type.OnTrue
                    || otherDNode.type == DialogueDataNode.Type.OnFalse)
                    return false;
                if (connectionCount > (isNewConnection ? 0 : 1))
                    return false;
                break;
            case DialogueDataNode.Type.Text:
                if (otherDNode.type == DialogueDataNode.Type.Answer
                    || otherDNode.type == DialogueDataNode.Type.OnTrue
                    || otherDNode.type == DialogueDataNode.Type.OnFalse)
                    return false;
                if (connectionCount > (isNewConnection ? 0 : 1))
                    return false;
                break;
            case DialogueDataNode.Type.Question:
                if (otherDNode.type != DialogueDataNode.Type.Answer || connectionCount > (isNewConnection ? 3 : 4))
                    return false;
                break;
            case DialogueDataNode.Type.Answer:
                if (otherDNode.type == DialogueDataNode.Type.Answer
                    || otherDNode.type == DialogueDataNode.Type.OnTrue
                    || otherDNode.type == DialogueDataNode.Type.OnFalse)
                    return false;
                if (connectionCount > (isNewConnection ? 0 : 1))
                    return false;
                break;
            case DialogueDataNode.Type.Condition:
                if (otherDNode.type != DialogueDataNode.Type.OnTrue && otherDNode.type != DialogueDataNode.Type.OnFalse)
                    return false;
                if (connectionCount > (isNewConnection ? 1 : 2))
                    return false;

                if (isNewConnection)
                {
                    if (connections.Count > 0)
                    {
                        DialogueDataNode left = (DialogueDataNode)connections[0];
                        if (left.type == otherDNode.type)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (connections.Count > 1)
                    {
                        DialogueDataNode left = (DialogueDataNode)connections[0];
                        DialogueDataNode right = (DialogueDataNode)connections[1];
                        if (left.type == right.type)
                        {
                            return false;
                        }
                    }
                }
                break;
            case DialogueDataNode.Type.OnTrue:
                if (otherDNode.type == DialogueDataNode.Type.Answer
                    || otherDNode.type == DialogueDataNode.Type.OnTrue 
                    || otherDNode.type == DialogueDataNode.Type.OnFalse)
                    return false;
                if (connectionCount > (isNewConnection ? 0 : 1))
                    return false;
                break;
            case DialogueDataNode.Type.OnFalse:
                if (otherDNode.type == DialogueDataNode.Type.Answer
                    || otherDNode.type == DialogueDataNode.Type.OnTrue
                    || otherDNode.type == DialogueDataNode.Type.OnFalse)
                    return false;
                if (connectionCount > (isNewConnection ? 0 : 1))
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

    protected override void UpdateSize()
    {
        base.UpdateSize();

        width = 300;
        
        switch(((DialogueDataNode)dataNode).type)
        {
            case DialogueDataNode.Type.StartDialogue:
            case DialogueDataNode.Type.EndDialogue:
            case DialogueDataNode.Type.OnTrue:
            case DialogueDataNode.Type.OnFalse:
                height = 125;
                break;
            case DialogueDataNode.Type.Text:
            case DialogueDataNode.Type.Question:
                height = 155;
                break;
            case DialogueDataNode.Type.Answer:
            case DialogueDataNode.Type.Condition:
                height = 220;
                break;
            
        }
    }
}
