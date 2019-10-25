using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ActionNode : Node
{
    public Action action;

    public ConnectionPoint inPoint;

    private GameObject _goSource;
    private float _offset = 10f;

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
                    if (_goSource.GetComponent<IDecision>() != null)// ACA DEBERIA DESPLEGAR LAS FUNCIONES QUE TIENE EL OBJETO
                    {
                        action = null; // PONER ACA LA FUNCION SELECCIONADA
                    }
                }
            }
            GUI.EndGroup();
        }

        base.Draw();
    }
}
