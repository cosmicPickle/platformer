using System;
using UnityEditor;
using UnityEngine;

public class Node
{
    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public float width = 200;
    public float height = 50;
    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;

    public Action<Node> OnRemoveNode;

    public DataGraphNode dataNode;
    public NodeBasedEditor currentEditor;
    public Editor inspector;

    public Node(
        Vector2 position, 
        Action<ConnectionPoint> OnClickInPoint,
        Action<ConnectionPoint> OnClickOutPoint,
        Action<Node> OnClickRemoveNode,
        DataGraphNode newDataNode,
        NodeBasedEditor editor
    )
    {
        dataNode = newDataNode;
        currentEditor = editor;

        SetStyles();
        UpdateSize();

        rect = new Rect(position.x, position.y, width, height);       
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, OnClickOutPoint);
        OnRemoveNode = OnClickRemoveNode;

        if(dataNode.uiSettings != null)
        {
            rect = dataNode.uiSettings.rect;
        } else
        {
            dataNode.SetUIRect(rect);
        }

        inspector = Editor.CreateEditor(dataNode);
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();

        UpdateSize();
        rect.width = width;
        rect.height = height;

        GUILayout.BeginArea(rect, style);
        inspector.OnInspectorGUI();
        GUILayout.EndArea();

        dataNode.SetUIRect(rect);
    }

    public bool ProcessEvents(Event e)
    {
        switch(e.type)
        {
            case EventType.MouseDown: 
                if(e.button == 0)
                {
                    if(rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    } else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }

                break;
            case EventType.MouseUp:
                isDragged = false;
                break;
            case EventType.MouseDrag:
                if(e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        return false;
    }

    public virtual bool IsConnectionAllowed(Node other, bool isNewConnection = false)
    {
        return true;
    }

    public virtual bool IsNodeAllowed(bool isNewNode = false)
    {
        return true;
    }

    protected virtual void SetStyles()
    {
        defaultNodeStyle = new GUIStyle();
        defaultNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
        defaultNodeStyle.border = new RectOffset(12, 12, 12, 12);
        defaultNodeStyle.padding = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        selectedNodeStyle.padding = new RectOffset(12, 12, 12, 12);

        style = defaultNodeStyle;
    }

    protected virtual void UpdateSize()
    {

    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }
}