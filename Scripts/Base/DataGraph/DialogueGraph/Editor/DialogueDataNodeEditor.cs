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

        if (dNode.type != DialogueDataNode.Type.Condition)
        {
            EditorGUILayout.LabelField("Text");
            dNode.text = EditorGUILayout.TextArea(dNode.text);
        }

        if (dNode.type == DialogueDataNode.Type.Condition)
        {
            EditorGUILayout.LabelField("Condition");
            dNode.condition = (DialogueDataNode.Condition)EditorGUILayout.EnumPopup(dNode.condition);

            if (dNode.condition == DialogueDataNode.Condition.HasItem)
            {
                EditorGUILayout.LabelField("ItemID");
                dNode.itemId = EditorGUILayout.TextField(dNode.itemId);
            }

            if (dNode.condition == DialogueDataNode.Condition.QuestStage)
            {
                EditorGUILayout.LabelField("QuestStageID");
                dNode.questStageId = EditorGUILayout.TextField(dNode.questStageId);
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}
