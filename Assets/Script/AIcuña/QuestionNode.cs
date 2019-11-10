using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

[Serializable]
public class QuestionNode : Node
{
    public ConnectionPoint inPoint;
    public ConnectionPoint truePoint;
    public ConnectionPoint falsePoint;

    public string goName;
    public string scriptName;
    public string methodName;

    [NonSerialized] public Func<bool> question;

    private Dictionary<string, object> unityDictionary = new Dictionary<string, object>();
    private List<MethodInfo> methodInfos = new List<MethodInfo>();
    public GameObject goSource;
    private int _scriptIndex;
    private int _methodIndex;

    private bool _isOpen;
    private Texture2D _openIcon;
    private Texture2D _closedIcon;
    private float _offset = 10f;

    public QuestionNode(Vector2 position, float width, float height,
        GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle truePointStyle, GUIStyle falsePointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnClickRemoveNode)
        : base(position, width, height, nodeStyle, selectedStyle, OnClickRemoveNode)
    {
        title = "New Question";
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        truePoint = new ConnectionPoint(this, ConnectionPointType.True, truePointStyle, OnClickOutPoint);
        falsePoint = new ConnectionPoint(this, ConnectionPointType.False, falsePointStyle, OnClickOutPoint);

        _isOpen = false;
        _openIcon = EditorGUIUtility.Load("icons/d_icon dropdown.png") as Texture2D;
        _closedIcon = EditorGUIUtility.Load("icons/icon dropdown.png") as Texture2D;
    }

    public QuestionNode(Vector2 position, float width, float height,
        GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle truePointStyle, GUIStyle falsePointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnClickRemoveNode,
        string nodeID, string inPointID, string truePointID, string falsePointID)
        : base(position, width, height, nodeStyle, selectedStyle, OnClickRemoveNode, nodeID)
    {
        title = "New Question";
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint, inPointID);
        truePoint = new ConnectionPoint(this, ConnectionPointType.True, truePointStyle, OnClickOutPoint, truePointID);
        falsePoint = new ConnectionPoint(this, ConnectionPointType.False, falsePointStyle, OnClickOutPoint, falsePointID);

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
                    title = EditorGUILayout.TextField(new GUIContent("Title", "Node title."), title);

                    goSource = (GameObject)EditorGUILayout.ObjectField(goSource, typeof(GameObject), true);

                    if (goSource != null)
                    {
                        goName = goSource.name;
                        List<object> targets = goSource.GetComponents<Component>().ToList<object>();

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

                    EditorGUI.BeginDisabledGroup(goSource == null);
                    {
                        EditorGUIUtility.labelWidth = 50;
                        _scriptIndex = EditorGUILayout.Popup("Script", _scriptIndex, options, EditorStyles.popup);

                        if (_scriptIndex != 0)
                        {
                            scriptName = scripsList[_scriptIndex];
                            methodInfos = GetMethod(unityDictionary[scripsList[_scriptIndex]]);

                            foreach (var methodsComp in GetMethod(unityDictionary[scripsList[_scriptIndex]]))
                            {
                                methodsList.Add(methodsComp.Name);
                            }
                        }

                        string[] optionsMethod = methodsList.ToArray();

                        EditorGUI.BeginDisabledGroup(_scriptIndex == 0);
                        {
                            _methodIndex = EditorGUILayout.Popup("Method", _methodIndex, optionsMethod, EditorStyles.popup);

                            if (_methodIndex != 0)
                            {
                                //question = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), unityDictionary[scripsList[_scriptIndex]], methodInfos[_methodIndex - 1].Name);
                                methodName = methodInfos[_methodIndex - 1].Name;
                                question = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), GameObject.Find(goName).GetComponent(scriptName) as MonoBehaviour, methodName); 

                            }
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndVertical();
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

    private void FullDictionary(List<object> targets)
    {
        if (targets == null) return;
        foreach (var components in targets)
        {
            if (components.GetType().Name != "Transform")
            {
                if (!unityDictionary.ContainsKey(components.GetType().Name))
                {
                    unityDictionary.Add(components.GetType().Name, components);
                }
            }
        }
    }

    public void SelectScript()
    {
        int index = 0;
        _scriptIndex = 0;
        if (goName == "") return;

        goSource = GameObject.Find(goName);
        FullDictionary(goSource.GetComponents<Component>().ToList<object>());

        if (unityDictionary != null)
        {
            foreach (var entry in unityDictionary)
            {
                index++;
                if (entry.Key == scriptName)
                {
                    _scriptIndex = index;
                }
            }
        }
    }

    public void SelectMethod()
    {
        int index = 0;
        _methodIndex = 0;
        if ((methodName == "")) return;
        if ((scriptName == "")) return;
        foreach (var methodsComp in GetMethod(unityDictionary[scriptName]))
        {
            index++;
            if (methodsComp.Name == methodName)
            {
                _methodIndex = index;
            }
        }
    }
}
