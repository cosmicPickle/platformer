using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DialogueDataNode : DataGraphNode {

    public enum Type { Text, Question, Answer, Condition, StartDialogue, EndDialogue };
    public enum Condition { None, HasItem, QuestStage }

    [SerializeField]
    public Type type;
    [SerializeField]
    public Condition condition;
    [SerializeField]
    public string itemId;
    [SerializeField]
    public string questStageId;
    [SerializeField]
    public string text;
    
}


