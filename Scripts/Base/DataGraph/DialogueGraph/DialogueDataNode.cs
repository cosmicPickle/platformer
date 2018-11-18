using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DialogueDataNode : DataGraphNode {

    public enum Type { Text, Question, Answer, Condition, StartDialogue, EndDialogue, OnTrue, OnFalse };
    public enum Action { None, AddItem }
    public enum Condition { None, HasItem }

    [SerializeField]
    public Type type;
    [SerializeField]
    public Condition condition;
    [SerializeField]
    public Action actionOnComplete;

    [SerializeField]
    public Item conditionItem;
    [SerializeField]
    public Item actionItem;
    [SerializeField]
    public string text;
    
}


