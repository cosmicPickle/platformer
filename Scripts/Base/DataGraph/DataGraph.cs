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

    public virtual int AddNode(DataGraphNode n)
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

