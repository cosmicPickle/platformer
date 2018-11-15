using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New default DataGraph", menuName = "DataGraph/Default", order = 0)]
[Serializable]
public class DataGraph : ScriptableObject
{
    [SerializeField]
    public int Size
    {
        get
        {
            return nodes.Count;
        }
    }

    [SerializeField]
    public List<DataGraphNode> nodes;
    [SerializeField]
    private List<DataGraphNodeConnection> nodeChildren;

    public DataGraph()
    {
        nodes = new List<DataGraphNode>();
        nodeChildren = new List<DataGraphNodeConnection>();
    }

    public int AddNode(DataGraphNode n)
    {
        int index = nodes.IndexOf(n);

        if (index < 0)
        {
            nodes.Add(n);
            nodeChildren.Add(new DataGraphNodeConnection());

            Debug.Assert(nodes.Count == nodeChildren.Count);

            return nodes.Count - 1;

        } else
        {
            return -1;
        }
    }

    public void RemoveNode(DataGraphNode n)
    {
        int index = nodes.IndexOf(n);

        if (index >= 0)
        {
            nodeChildren.RemoveAt(index);
            nodes.Remove(n);
        }
    }

    public bool AddEdge(DataGraphNode n, DataGraphNode v)
    {
        int nIndex = nodes.IndexOf(n);

        if (nIndex < 0)
        {
            nIndex = AddNode(n);
        }

        if (!nodes.Contains(v))
        {
            AddNode(v);
        }

        if(nIndex < 0)
        {
            return false;
        }

        if (!nodeChildren[nIndex].list.Contains(v))
        {
            nodeChildren[nIndex].list.Add(v);
        }

        return true;
    }

    public void RemoveEdge(DataGraphNode n, DataGraphNode v)
    {
        int index = nodes.IndexOf(n);

        if (index >= 0)
        {
            nodeChildren[index].list.Remove(v);
        }
    }

    public List<DataGraphNode> GetNodeConnections(DataGraphNode n)
    {
        int index = nodes.IndexOf(n);

        if (index < 0)
            return null;


        return nodeChildren[index].list;
    }
  
}

[Serializable]
public class DataGraphNodeConnection
{
    [SerializeField]
    public List<DataGraphNode> list;

    public DataGraphNodeConnection()
    {
        list = new List<DataGraphNode>();
    }
}

[Serializable]
public class DataGraphNode
{
    [SerializeField]
    string nodeId;

    [SerializeField]
    public UISettings uiSettings = null;

    public DataGraphNode()
    {
        nodeId = Guid.NewGuid().ToString();
    }

    public void SetUIRect(Rect rect)
    {
        if(uiSettings == null)
        {
            uiSettings = new UISettings();
        }

        uiSettings.rect = rect;
    }

    public bool CanConnectWith(DataGraphNode n)
    {
        return false;
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

    public bool Equals(DataGraphNode obj)
    {
        return nodeId == obj.nodeId;
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
