using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNodeBasedEditor : NodeBasedEditor {

    protected override Node BindNode(Vector2 mousePosition, DataGraphNode dataGraphNode)
    {
        return new DialogueNode(
            mousePosition,
            OnClickInPoint,
            OnClickOutPoint,
            OnClickRemoveNode,
            dataGraphNode,
            this
        );
    }
}
