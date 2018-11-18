using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : Interactable {

    public DialogueDataGraph dialog;

    bool conversationStarted;
    DialogueDataNode currentConversationNode;

    protected override void Interact()
    {
        base.Interact();

        if(!conversationStarted && dialog.Size > 0)
        {
            Debug.Log("Conversation Stated");
            currentConversationNode = (DialogueDataNode)dialog.startDialogue;

            if(currentConversationNode == null)
            {
                return;
            }

            conversationStarted = true;
            OnNext();
        }
    }

    protected virtual void ProcessCurrentNode()
    {
        switch (currentConversationNode.type) {
            case DialogueDataNode.Type.Text:
                ConversationUI.instance.ShowTextNode(currentConversationNode, OnNext);
                ConversationUI.instance.Show();
                break;
            case DialogueDataNode.Type.Question:
                ConversationUI.instance.ShowQuestionNode(currentConversationNode, FilterConditionalAnswers(dialog.GetNodeConnections(currentConversationNode)), OnAnswer);
                ConversationUI.instance.Show();
                break;
            case DialogueDataNode.Type.Condition:
                OnConditionNode(ProcessCondition(currentConversationNode));
                break;
            case DialogueDataNode.Type.EndDialogue:
                ConversationUI.instance.Hide();
                conversationStarted = false;
                break;
        }
    }

    protected virtual List<DataGraphNode> FilterConditionalAnswers(List<DataGraphNode> answers)
    {
        List<DataGraphNode> filtered = new List<DataGraphNode>();

        answers.ForEach(answer =>
        {
            if(ProcessCondition((DialogueDataNode)answer))
            {
                filtered.Add(answer);
            }
        });

        return filtered;
    }

    protected virtual bool ProcessCondition(DialogueDataNode node)
    {
        switch (node.condition)
        {
            case DialogueDataNode.Condition.None:
                return true;
            case DialogueDataNode.Condition.HasItem:
                return Inventory.instance.HasItem(node.conditionItem);
        }

        Debug.LogWarning("Unprocessed Condition");
        return false;
    }

    protected virtual void ProcessAction(DialogueDataNode node)
    {
        switch (node.actionOnComplete)
        {
            case DialogueDataNode.Action.None:
                return;
            case DialogueDataNode.Action.AddItem:
                Inventory.instance.Add(node.actionItem);
                break;
        }
    }

    protected virtual void OnNext()
    {
        if (currentConversationNode.type != DialogueDataNode.Type.Text && currentConversationNode.type != DialogueDataNode.Type.StartDialogue)
        {
            return;
        }

        List<DataGraphNode> connections = dialog.GetNodeConnections(currentConversationNode);

        ProcessAction(currentConversationNode);

        currentConversationNode = (DialogueDataNode)connections[0];
        ProcessCurrentNode();
    }

    protected virtual void OnAnswer(DialogueDataNode answer)
    {
        if (currentConversationNode.type != DialogueDataNode.Type.Question)
        {
            return;
        }

        ProcessAction(currentConversationNode);
        ProcessAction(answer);

        List<DataGraphNode> connections = dialog.GetNodeConnections(answer);

        currentConversationNode = (DialogueDataNode)connections[0];
        ProcessCurrentNode();
    }

    protected virtual void OnConditionNode(bool result)
    {
        List<DataGraphNode> connections = dialog.GetNodeConnections(currentConversationNode);

        if(connections.Count < 2)
        {
            Debug.LogWarning("Dialogue condition node is not setup corectly");
        }

        ProcessAction(currentConversationNode);

        DialogueDataNode onTrue = (DialogueDataNode)connections[0];
        DialogueDataNode onFalse = onTrue.type == DialogueDataNode.Type.OnFalse ? onTrue : (DialogueDataNode)connections[1];

        if(onTrue == onFalse)
        {
            onTrue = (DialogueDataNode)connections[1];
        }

        if(result)
        {
            ProcessAction(onTrue);

            currentConversationNode = (DialogueDataNode)dialog.GetNodeConnections(onTrue)[0];
        } else
        {
            ProcessAction(onFalse);

            currentConversationNode = (DialogueDataNode)dialog.GetNodeConnections(onFalse)[0];
        }
        ProcessCurrentNode();
    }
}
