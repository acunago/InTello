using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

[Serializable]
public class ActionNode : Node
{
    public ConnectionPoint inPoint;

    public string goName;
    public string scriptName;
    public string methodName;

    [NonSerialized] public Action action;

    private Dictionary<string, object> unityDictionary = new Dictionary<string, object>();
    private List<MethodInfo> methodInfos = new List<MethodInfo>();
    private GameObject _goSource;
    private int _scriptIndex;
    private int _methodIndex;

    private bool _isOpen;
    private Texture2D _openIcon;
    private Texture2D _closedIcon;
    private float _offset = 10f;

    public ActionNode(Vector2 position, float width, float height,
        GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<Node> OnClickRemoveNode)
        : base(position, width, height, nodeStyle, selectedStyle, OnClickRemoveNode)
    {
        title = "New Action";
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);

        _isOpen = false;
        _openIcon = EditorGUIUtility.Load("icons/d_icon dropdown.png") as Texture2D;
        _closedIcon = EditorGUIUtility.Load("icons/icon dropdown.png") as Texture2D;
    }

    public ActionNode(Vector2 position, float width, float height,
        GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle,
        Action<ConnectionPoint> OnClickInPoint, Action<Node> OnClickRemoveNode, 
        string nodeID, string inPointID)
        : base(position, width, height, nodeStyle, selectedStyle, OnClickRemoveNode, nodeID)
    {
        title = "New Action";
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint, inPointID);

        _isOpen = false;
        _openIcon = EditorGUIUtility.Load("icons/d_icon dropdown.png") as Texture2D;
        _closedIcon = EditorGUIUtility.Load("icons/icon dropdown.png") as Texture2D;
    }

    public override void Draw()
    {
        inPoint.Draw();

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

                    _goSource = (GameObject)EditorGUILayout.ObjectField(_goSource, typeof(GameObject), true);

                    if (_goSource != null)
                    {
                        goName = _goSource.name;
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
                                methodName = methodInfos[_methodIndex - 1].Name;
                                try
                                {
                                    action = (Action)Delegate.CreateDelegate(typeof(Action), GameObject.Find(goName).GetComponent(scriptName) as MonoBehaviour, methodName);
                                }
                                catch (Exception)
                                {
                                    EditorGUILayout.HelpBox(methodName + " is not a Method", MessageType.Error);
                                    _methodIndex = 0;
                     
                                }
                                
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
        Debug.Log(goName);
        if (goName == "") return;
        if (scriptName == "") return;

        _goSource = GameObject.Find(goName);
        if (_goSource == null) return;
        FullDictionary(_goSource.GetComponents<Component>().ToList<object>());

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
