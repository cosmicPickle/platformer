using System;
using System.Collections.Generic;
using UnityEngine;

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