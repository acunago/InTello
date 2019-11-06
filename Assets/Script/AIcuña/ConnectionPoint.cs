using System;
using UnityEngine;

[Serializable] 
public enum ConnectionPointType { In, True, False }
[Serializable]
public class ConnectionPoint : System.Object
{

    public string id;
    public ConnectionPointType type;

    public Rect rect;

    public Node node;

    public GUIStyle style;

    public Action<ConnectionPoint> OnClickConnectionPoint;

    public ConnectionPoint(Node node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint, string _id = null)
    {
        this.node = node;
        this.type = type;
        this.style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        rect = new Rect(0, 0, 15f, 20f);
        id = _id ?? Guid.NewGuid().ToString();
    }

    public void Draw()
    {
        rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;

        switch (type)
        {
            case ConnectionPointType.In:
                rect.x = node.rect.x - rect.width + 8f;
                break;

            case ConnectionPointType.True:
                rect.y -= rect.height / 2;
                rect.x = node.rect.x + node.rect.width - 8f;
                break;
            case ConnectionPointType.False:
                rect.y += rect.height / 2;
                rect.x = node.rect.x + node.rect.width - 8f;
                break;
        }

        if (GUI.Button(rect, "", style))
        {
            OnClickConnectionPoint?.Invoke(this);
        }
    }
}
