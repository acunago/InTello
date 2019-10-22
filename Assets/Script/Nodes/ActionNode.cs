using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ActionNode : Node
{
    public delegate void DesMethod();
    private DesMethod desDelegate;
    private GameObject goSource;
    private IDecision nodeDes;

    public ConnectionPoint inPoint;

    private float offset = 10f;

    public ActionNode(Vector2 position, float width, float height,
        GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<Node> OnClickRemoveNode)
        : base(position, width, height, nodeStyle, selectedStyle, OnClickRemoveNode)
    {
        name = "New Action";
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
    }

    public override void Draw()
    {
        inPoint.Draw();

        if (isSelected) // CAMBIAR POR TOGGLE EN EL BOX
        {
            Rect extra = new Rect(rect);
            extra.x += offset;
            extra.y += offset + rect.height / 2;
            extra.width -= 2 * offset;
            extra.height = 60f;
            GUI.BeginGroup(extra);
            {
                EditorGUI.DrawRect(new Rect(0, 0, extra.width, extra.height), new Color(0, 0, 0, .5f));
                EditorGUIUtility.labelWidth = 40;
                name = EditorGUILayout.TextField(new GUIContent("Name", "Node Name."), name);

                goSource = (GameObject)EditorGUILayout.ObjectField(goSource, typeof(GameObject), true);
                if (goSource != null)
                {
                    if (goSource.GetComponent<IDecision>() != null)
                    {
                        desDelegate = new DesMethod(goSource.GetComponent<IDecision>().Execute);
                    }
                }
            }
            GUI.EndGroup();
        }

        base.Draw();
    }
}
