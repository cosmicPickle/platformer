using System;
using UnityEditor;
using UnityEngine;

public enum ConnectionPointType { In, Out }

public class ConnectionPoint
{
    public Rect rect;
    public ConnectionPointType type;
    public Node node;
    public GUIStyle style;
    public Action<ConnectionPoint> OnClickConnectionPoint;

    public static GUIStyle inPointStyle;
    public static GUIStyle outPointStyle;

    public ConnectionPoint(Node node, ConnectionPointType type, Action<ConnectionPoint> OnClickConnectionPoint)
    {
        this.node = node;
        this.type = type;

        SetStyles();

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

    protected virtual void SetStyles()
    {
        if (inPointStyle == null)
        {
            inPointStyle = new GUIStyle();
            inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/btn.png") as Texture2D;
            inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/lightskin/images/btn on.png") as Texture2D;
            inPointStyle.border = new RectOffset(12, 12, 4, 4);
        }

        if (outPointStyle == null)
        {
            outPointStyle = new GUIStyle();
            outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/btn.png") as Texture2D;
            outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/lightskin/images/btn on.png") as Texture2D;
            outPointStyle.border = new RectOffset(12, 12, 4, 4);
        }

        style = type == ConnectionPointType.In ? inPointStyle : outPointStyle;
    }
}
