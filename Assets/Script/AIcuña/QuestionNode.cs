using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class QuestionNode : Node
{
    public Func<bool> question;

    public ConnectionPoint inPoint;
    public ConnectionPoint truePoint;
    public ConnectionPoint falsePoint;

    private GameObject _goSource;
    private float _offset = 10f;

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
            extra.x += _offset;
            extra.y += _offset + rect.height / 2;
            extra.width -= 2 * _offset;
            extra.height = 60f;
            GUI.BeginGroup(extra);
            {
                EditorGUI.DrawRect(new Rect(0, 0, extra.width, extra.height), new Color(0, 0, 0, .5f));
                EditorGUIUtility.labelWidth = 40;
                name = EditorGUILayout.TextField(new GUIContent("Name", "Node Name."), name);

                _goSource = (GameObject)EditorGUILayout.ObjectField(_goSource, typeof(GameObject), true);
                if (_goSource != null)
                {
                    // MEJORAR Y CORREGIR
                    if (_goSource.GetComponent<IDecision>() != null) // ACA DEBERIA DESPLEGAR LAS FUNCIONES QUE TIENE EL OBJETO
                    {
                        question = null; // PONER ACA LA FUNCION SELECCIONADA
                    }
                }
            }
            GUI.EndGroup();
        }

        base.Draw();
    }
}
