using System;
using UnityEngine;

public enum ConnectionPointType { In, Out }

public class ConnectionPoint
{
    public Rect rect;
    public ConnectionPointType type;
    public Node node;
    public GUIStyle style;
    public Action<ConnectionPoint> OnClickConnectionPoint;

    public ConnectionPoint(Node node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint)
    {
        this.node = node;
        this.type = type;
        this.style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        rect = new Rect(0, 0, 20f, 10f);
    }

    public void Draw()
    {
        rect.x = node.rect.x + (node.rect.width * 0.5f) - rect.width * 0.5f;

        switch(type)
        {
            case ConnectionPointType.In:
                rect.y = node.rect.y - rect.height + 8f;
                break;
            case ConnectionPointType.Out:
                rect.y = node.rect.y + node.rect.height - 8f;
                break;
        }

        if(GUI.Button(rect, "", style))
        {
            if(OnClickConnectionPoint != null)
            {
                OnClickConnectionPoint(this);
            }
        }
    }
}
