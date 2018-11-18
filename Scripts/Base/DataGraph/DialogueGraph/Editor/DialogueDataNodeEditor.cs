using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueDataNode))]
public class DialogueDataNodeEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DialogueDataNode dNode = (DialogueDataNode)target;

        dNode.type = (DialogueDataNode.Type)EditorGUILayout.EnumPopup(dNode.type);

        if (dNode.type != DialogueDataNode.Type.Condition 
            && dNode.type != DialogueDataNode.Type.StartDialogue 
            && dNode.type != DialogueDataNode.Type.EndDialogue
            && dNode.type != DialogueDataNode.Type.OnFalse
            && dNode.type != DialogueDataNode.Type.OnTrue)
        {
            EditorGUILayout.LabelField("Text");
            dNode.text = EditorGUILayout.TextArea(dNode.text);
        }

        if (dNode.type == DialogueDataNode.Type.Condition || dNode.type == DialogueDataNode.Type.Answer)
        {
            EditorGUILayout.LabelField("Condition");
            dNode.condition = (DialogueDataNode.Condition)EditorGUILayout.EnumPopup(dNode.condition);

            if (dNode.condition == DialogueDataNode.Condition.HasItem)
            {
                EditorGUILayout.LabelField("Item");
                dNode.conditionItem = EditorGUILayout.ObjectField(dNode.conditionItem, typeof(Item)) as Item;
            }
        }

        EditorGUILayout.LabelField("Action on Complete");
        dNode.actionOnComplete = (DialogueDataNode.Action)EditorGUILayout.EnumPopup(dNode.actionOnComplete);

        if (dNode.actionOnComplete == DialogueDataNode.Action.AddItem)
        {
            EditorGUILayout.LabelField("Item");
            dNode.actionItem = EditorGUILayout.ObjectField(dNode.actionItem, typeof(Item)) as Item;
        }
    }
}
