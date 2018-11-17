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
                ConversationUI.instance.ShowQuestionNode(currentConversationNode, dialog.GetNodeConnections(currentConversationNode), OnAnswer);
                ConversationUI.instance.Show();
                break;
            case DialogueDataNode.Type.EndDialogue:
                ConversationUI.instance.Hide();
                conversationStarted = false;
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

        currentConversationNode = (DialogueDataNode)connections[0];
        ProcessCurrentNode();
    }

    protected virtual void OnAnswer(DialogueDataNode answer)
    {
        if (currentConversationNode.type != DialogueDataNode.Type.Question)
        {
            return;
        }

        List<DataGraphNode> connections = dialog.GetNodeConnections(answer);

        currentConversationNode = (DialogueDataNode)connections[0];
        ProcessCurrentNode();
    }
}
