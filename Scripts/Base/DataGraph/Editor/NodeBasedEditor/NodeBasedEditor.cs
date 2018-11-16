using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class NodeBasedEditor : EditorWindow
{
    public delegate void OnDataChange();
    public OnDataChange onDataChange;

    private List<Node> nodes;
    private List<Connection> connections;

    protected GUIStyle nodeStyle;
    protected GUIStyle selectedNodeStyle;
    protected GUIStyle inPointStyle;
    protected GUIStyle outPointStyle;

    protected ConnectionPoint selectedInPoint;
    protected ConnectionPoint selectedOutPoint;

    protected Vector2 offset;
    protected Vector2 drag;
    protected static Texture2D tex;

    public delegate DataGraphNode OnCreateDataNodeAction();
    protected OnCreateDataNodeAction OnCreateDataNode;

    protected Action<DataGraphNode> OnRemoveDataNode;
    protected Action<Connection> OnCreateDataEdge;
    protected Action<Connection> OnRemoveDataEdge;

    protected DataGraph data;

    public DataGraph GetData()
    {
        return data;
    }

    public void SetData(
        ref DataGraph newData,
        OnCreateDataNodeAction OnCreateDataNode, 
        Action<DataGraphNode> OnRemoveDataNode, 
        Action<Connection> OnCreateDataEdge, 
        Action<Connection> OnRemoveDataEdge
    )
    {
        data = newData;
        GenerateNodesFromData();

        this.OnCreateDataNode = OnCreateDataNode;
        this.OnRemoveDataNode = OnRemoveDataNode;
        this.OnCreateDataEdge = OnCreateDataEdge;
        this.OnRemoveDataEdge = OnRemoveDataEdge;
    }

    public void ClearData()
    {
        data = null;
        OnCreateDataNode = null;
        OnRemoveDataNode = null;
        OnCreateDataEdge = null;
        OnRemoveDataEdge = null;

        ClearCanvas();
    }

    protected virtual Node BindNode(Vector2 mousePosition, DataGraphNode dataGraphNode)
    {
        return new Node(
            mousePosition,
            OnClickInPoint,
            OnClickOutPoint,
            OnClickRemoveNode,
            dataGraphNode,
            this
        );
    }

    [MenuItem("Window/Node Based Editor")]
    private static void OpenWindow()
    {
        NodeBasedEditor window = GetWindow<NodeBasedEditor>();
        window.titleContent = new GUIContent("Node Based Editor");
    }

    private void OnEnable()
    {
        tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, new Color(0.27f, 0.27f, 0.27f));
        tex.Apply();

    }

    private void OnGUI()
    {
        if (tex == null)
        {
            tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, new Color(0.27f, 0.27f, 0.27f));
            tex.Apply();
        }

        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), tex, ScaleMode.StretchToFill);

        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        if(!data)
        {
            GUILayout.Label("No DataGraph selected.", EditorStyles.boldLabel);
        } else
        {
            GUILayout.Label("Editing DataGraph: " + data.name, EditorStyles.boldLabel);
        }

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    private void GenerateNodesFromData()
    {
        Dictionary<DataGraphNode, Node> nodeMap = new Dictionary<DataGraphNode, Node>();

        data.nodes.ForEach((DataGraphNode n) =>
        {
            nodeMap.Add(n, OnClickAddNode(n.uiSettings.rect.position, n));
        });

        if (nodes != null)
        {
            nodes.ForEach((Node n) =>
            {
                List<DataGraphNode> dataNodeConnections = data.GetNodeConnections(n.dataNode);

                dataNodeConnections.ForEach((DataGraphNode c) =>
                {
                    if(connections == null)
                    {
                        connections = new List<Connection>();
                    }

                    connections.Add(new Connection(
                        nodeMap[c].inPoint,
                        n.outPoint,
                        OnClickRemoveConnection
                    ));
                });
            });
        }
    }

    private void ClearCanvas()
    {
        nodes = null;
        connections = null;
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    private void DrawNodes()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if(selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();

    }

    protected virtual void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;
            case EventType.MouseDrag:
                if(e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    protected virtual void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;

                    if (onDataChange != null)
                    {
                        onDataChange();
                    }
                }
            }
        }
    }

    protected virtual void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }


    protected virtual Node OnClickAddNode(Vector2 mousePosition, DataGraphNode dataNode = null)
    {
        if (data == null)
        {
            return null;
        }

        if (nodes == null)
        {
            nodes = new List<Node>();
        }
        
        DataGraphNode dataGraphNode = dataNode != null ? dataNode : OnCreateDataNode();
        Node newNode = BindNode(mousePosition, dataGraphNode);

        nodes.Add(newNode);

        return newNode;
    }

    protected virtual void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        if(selectedOutPoint != null)
        {
            if(selectedInPoint.node != selectedOutPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            } else
            {
                ClearConnectionSelection();
            }
        }
    }

    protected virtual void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    protected virtual void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
        OnRemoveDataEdge(connection);
    }

    protected virtual void OnDrag(Vector2 delta)
    {
        drag = delta;

        if(nodes != null)
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        if (onDataChange != null)
        {
            onDataChange();
        }

        GUI.changed = true;
    }

    protected virtual void CreateConnection()
    {
        if(connections == null)
        {
            connections = new List<Connection>();
        }

        if (selectedOutPoint.node.IsConnectionAllowed(selectedInPoint.node))
        {
            Connection conn = new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection);
            connections.Add(conn);

            OnCreateDataEdge(conn);
        }
    }

    protected virtual void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    protected virtual void OnClickRemoveNode(Node node)
    {
        if (connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        OnRemoveDataNode(node.dataNode);
        nodes.Remove(node);
    }
}