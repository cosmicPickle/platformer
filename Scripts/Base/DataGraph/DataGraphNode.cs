using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class DataGraphNode : ScriptableObject
{
    [SerializeField]
    [HideInInspector]
    string nodeId;

    [SerializeField]
    [HideInInspector]
    public UISettings uiSettings = null;

    public DataGraphNode()
    {
        nodeId = Guid.NewGuid().ToString();
    }

    public void SetUIRect(Rect rect)
    {
        if (uiSettings == null)
        {
            uiSettings = new UISettings();
        }

        uiSettings.rect = rect;
    }

    public static bool operator ==(DataGraphNode n, DataGraphNode v)
    {
        if ((object)v == null && (object)n == null)
            return true;
        else if ((object)n == null || (object)v == null)
            return false;
        else if (n.nodeId == v.nodeId)
            return true;
        else
            return false;
    }

    public static bool operator !=(DataGraphNode n, DataGraphNode v)
    {
        if ((object)v == null && (object)n == null)
            return false;
        else if ((object)n == null || (object)v == null)
            return true;
        else if (n.nodeId == v.nodeId)
            return false;
        else
            return true;
    }

    public override int GetHashCode()
    {
        var hashCode = -876610690;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(nodeId);
        return hashCode;
    }

    public override bool Equals(object obj)
    {
        return nodeId == ((DataGraphNode)obj).nodeId;
    }

    [System.Serializable]
    public class UISettings
    {
        [SerializeField]
        public Rect rect;
    }

    public class EqualityComparer : IEqualityComparer<DataGraphNode>
    {
        public bool Equals(DataGraphNode n, DataGraphNode v)
        {
            if (v == null && n == null)
                return true;
            else if (n == null || v == null)
                return false;
            else if (n.nodeId == v.nodeId)
                return true;
            else
                return false;
        }

        public int GetHashCode(DataGraphNode n)
        {
            return n.nodeId.GetHashCode();
        }
    }
}
