using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class QuestionNode : Node
{

    public ConnectionPoint inPoint;
    public ConnectionPoint truePoint;
    public ConnectionPoint falsePoint;

    public QuestionNode(Vector2 position, float width, float height,
        GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle truePointStyle, GUIStyle falsePointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnClickRemoveNode)
        : base(position, width, height, nodeStyle, selectedStyle, OnClickRemoveNode)
    {
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        truePoint = new ConnectionPoint(this, ConnectionPointType.True, truePointStyle, OnClickOutPoint);
        falsePoint = new ConnectionPoint(this, ConnectionPointType.False, falsePointStyle, OnClickOutPoint);
    }

    public override void Draw()
    {
        inPoint.Draw();
        truePoint.Draw();
        falsePoint.Draw();

        base.Draw();
    }
}
