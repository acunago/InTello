using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using UnityEngine.Events;

public class ActionNode : Node
{
    public Action action;

    public ConnectionPoint inPoint;

    private GameObject _goSource;
    private float _offset = 10f;

    Dictionary<string, UnityAction> unityDictionary = new Dictionary<string, UnityAction>();
    private UnityAction myDel;
    private List<MethodInfo> methodInfos = new List<MethodInfo>();

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

        List<string> lista = new List<string>();
        lista.Add("");

        if (isSelected) // CAMBIAR POR TOGGLE EN EL BOX
        {
            Rect extra = new Rect(rect);
            extra.x += _offset;
            extra.y += _offset + rect.height / 2;
            extra.width -= 2 * _offset;
            extra.height = 90f;
            GUI.BeginGroup(extra);
            {
                EditorGUI.DrawRect(new Rect(0, 0, extra.width, extra.height), new Color(0, 0, 0, .5f));
                EditorGUIUtility.labelWidth = 40;
                name = EditorGUILayout.TextField(new GUIContent("Name", "Node Name."), name);

                _goSource = (GameObject)EditorGUILayout.ObjectField(_goSource, typeof(GameObject), true);
                if (_goSource != null)
                {
                   List<object> targets = _goSource.GetComponents<Component>().ToList<System.Object>();


                    foreach (var components in (targets))
                    {

                        if (components.GetType().Name != "Transform")
                        {
                            lista.Add(components.GetType().Name);

                            foreach (var methodsComp in GetMethod(components))
                            {
                                //Debug.Log(" nombre " + methodsComp.Name);
                                if (methodsComp.Name == "Comer")
                                {
                                    action = (Action)Delegate.CreateDelegate(typeof(Action), components, methodsComp.Name);
                                }

                            }
                        }
                    }



                    int selected = 0;
                    string[] options = lista.ToArray();
                    selected = EditorGUILayout.Popup("Scripts", selected, options);


                    action.Invoke();
                }

            }
            GUI.EndGroup();
        }



        base.Draw();
    }

    public List<MethodInfo> GetMethod(object target)
    {
        return target.GetType().GetMethods().Where(x=>x.DeclaringType.Equals(target.GetType())).ToList();

    }
}
