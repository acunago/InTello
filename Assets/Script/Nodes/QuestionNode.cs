using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class QuestionNode : Node
{
    public delegate void QuestMethod(IDecision trueNode, IDecision falseNode);
    private QuestMethod quesDelegate;
    private GameObject goSource;
    private IQuestion nodeQues;

    public ConnectionPoint inPoint;
    public ConnectionPoint truePoint;
    public ConnectionPoint falsePoint;

    private float offset = 10f;

    public QuestionNode(Vector2 position, float width, float height,
        GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle truePointStyle, GUIStyle falsePointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnClickRemoveNode)
        : base(position, width, height, nodeStyle, selectedStyle, OnClickRemoveNode)
    {
        name = "New Question";
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        truePoint = new ConnectionPoint(this, ConnectionPointType.True, truePointStyle, OnClickOutPoint);
        falsePoint = new ConnectionPoint(this, ConnectionPointType.False, falsePointStyle, OnClickOutPoint);
    }

    public override void Draw()
    {
        inPoint.Draw();
        truePoint.Draw();
        falsePoint.Draw();

        if (isSelected) // CAMBIAR POR TOGGLE EN EL BOX
        {
            Rect extra = new Rect(rect);
            extra.x += offset;
            extra.y += offset + rect.height / 2;
            extra.width -= 2* offset;
            extra.height = 60f;
            GUI.BeginGroup(extra);
            {
                EditorGUI.DrawRect(new Rect(0, 0, extra.width, extra.height), new Color(0, 0, 0, .5f));
                EditorGUIUtility.labelWidth = 40;
                name = EditorGUILayout.TextField(new GUIContent("Name", "Node Name."), name);

                goSource = (GameObject)EditorGUILayout.ObjectField(goSource, typeof(GameObject), true);
                if (goSource != null)
                {
                    if (goSource.GetComponent<IQuestion>() != null)
                    {
                        quesDelegate = new QuestMethod(goSource.GetComponent<IQuestion>().Execute);
                    }
                }
            }
            GUI.EndGroup();
        }

        base.Draw();
    }
}
