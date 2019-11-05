using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

[Serializable]
public class QuestionNode : Node
{
    public Func<bool> question;

    public ConnectionPoint inPoint;
    public ConnectionPoint truePoint;
    public ConnectionPoint falsePoint;

    private GameObject _goSource;

    private Dictionary<string, object> unityDictionary = new Dictionary<string, object>();
    private Func<bool> myDel;
    private List<MethodInfo> methodInfos = new List<MethodInfo>();

    private int selectedScript;
    private int selectedMethod;

    private bool _isOpen;
    private Texture2D _openIcon;
    private Texture2D _closedIcon;
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

        _isOpen = false;
        _openIcon = EditorGUIUtility.Load("icons/d_icon dropdown.png") as Texture2D;
        _closedIcon = EditorGUIUtility.Load("icons/icon dropdown.png") as Texture2D;
    }

    public override void Draw()
    {
        inPoint.Draw();
        truePoint.Draw();
        falsePoint.Draw();

        List<string> scripsList = new List<string>();
        List<string> methodsList = new List<string>();
        scripsList.Add("");
        methodsList.Add("");

        if (_isOpen)
        {
            Rect extra = new Rect(rect);
            extra.x += _offset;
            extra.y += rect.height - _offset;
            extra.width -= 2 * _offset;
            extra.height = 80f;
            GUILayout.BeginArea(extra);
            {
                EditorGUI.DrawRect(new Rect(0, 0, extra.width, extra.height), new Color(0, 0, 0, .5f));

                EditorGUILayout.BeginVertical();
                {
                    GUILayout.Space(5);
                    EditorGUIUtility.labelWidth = 40;
                    name = EditorGUILayout.TextField(new GUIContent("Name", "Node Name."), name);

                    _goSource = (GameObject)EditorGUILayout.ObjectField(_goSource, typeof(GameObject), true);

                    if (_goSource != null)
                    {
                        List<object> targets = _goSource.GetComponents<Component>().ToList<object>();

                        foreach (var components in targets)
                        {
                            if (components.GetType().Name != "Transform")
                            {
                                if (!unityDictionary.ContainsKey(components.GetType().Name))
                                {
                                    unityDictionary.Add(components.GetType().Name, components);
                                }
                                scripsList.Add(components.GetType().Name);
                            }
                        }
                    }

                    string[] options = scripsList.ToArray();

                    EditorGUI.BeginDisabledGroup(_goSource == null);
                    {
                        EditorGUIUtility.labelWidth = 50;
                        selectedScript = EditorGUILayout.Popup("Script", selectedScript, options, EditorStyles.popup);

                        if (selectedScript != 0)
                        {
                            methodInfos = GetMethod(unityDictionary[scripsList[selectedScript]]);

                            foreach (var methodsComp in GetMethod(unityDictionary[scripsList[selectedScript]]))
                            {
                                methodsList.Add(methodsComp.Name);
                            }
                        }

                        string[] optionsMethod = methodsList.ToArray();

                        EditorGUI.BeginDisabledGroup(selectedScript == 0);
                        {
                            selectedMethod = EditorGUILayout.Popup("Method", selectedMethod, optionsMethod, EditorStyles.popup);

                            if (selectedMethod != 0)
                            {
                                question = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), unityDictionary[scripsList[selectedScript]], methodInfos[selectedMethod - 1].Name);
                            }
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndVertical();

                /*esto es para test
                if (question != null)
                {
                    question.Invoke();
                }*/
            }
            GUILayout.EndArea();
        }

        base.Draw();

        Rect btn = new Rect(rect);
        btn.x += 8f;
        btn.y += rect.height - 16f;
        btn.width -= 16f;
        btn.height = 8f;

        if (GUI.Button(btn, (_isOpen) ? _openIcon : _closedIcon))
            _isOpen = !_isOpen;
    }

    public List<MethodInfo> GetMethod(object target)
    {
        return target.GetType().GetMethods().Where(x => x.DeclaringType.Equals(target.GetType())).ToList();
    }
}
