using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

public enum OPTIONS
{
    ACTION = 0,
    QUESTION = 1,
    TREE = 2
}

public class NodeEditor : EditorWindow
{
    string _mName;
    List<BaseNode> _mNodes;
    string _nodeName;

    BaseNode _selectedNode;

    public OPTIONS op; // MUY POSIBLEMENTE LO HAGA VOLAR
    string toLink = ""; // REVISAR COMO CORREGIR ESTO

    // A REVISAR POR PANEO
    private bool _panningScreen;
    private Vector2 graphPan;
    private Vector2 _originalMousePosition;
    private Vector2 prevPan;
    private Rect graphRect;

    // A REVISAR POR DISEÑO
    private float toolbarHeight = 100;
    public GUIStyle wrapTextFieldStyle;

    [MenuItem("InTello/AIcuña")]
    public static void OpenWindow()
    {
        var mySelf = GetWindow<NodeEditor>();
        mySelf.graphPan = new Vector2(0, mySelf.toolbarHeight);
        mySelf.graphRect = new Rect(0, mySelf.toolbarHeight, 1000000, 1000000);

        mySelf.wrapTextFieldStyle = new GUIStyle(EditorStyles.textField);
        mySelf.wrapTextFieldStyle.wordWrap = true;
    }

    void OnEnable()
    {
        _mName = "";
        _mNodes = new List<BaseNode>();
    }

    void OnGUI()
    {
        CheckMouseInput(Event.current);

        DrawToolbar();
        DrawWorkSpace();

    }

    // REVISAR PARA EL PANEO
    void CheckMouseInput(Event currentE)
    {
        if (!graphRect.Contains(currentE.mousePosition) || !(focusedWindow == this || mouseOverWindow == this))
            return;

        if (currentE.button == 2 && currentE.type == EventType.MouseDown)
        {
            _panningScreen = true;
            prevPan = new Vector2(graphPan.x, graphPan.y);
            _originalMousePosition = currentE.mousePosition;
        }
        else if (currentE.button == 2 && currentE.type == EventType.MouseUp)
            _panningScreen = false;

        if (_panningScreen)
        {
            var newX = prevPan.x + currentE.mousePosition.x - _originalMousePosition.x;
            graphPan.x = newX > 0 ? 0 : newX;

            var newY = prevPan.y + currentE.mousePosition.y - _originalMousePosition.y;
            graphPan.y = newY > toolbarHeight ? toolbarHeight : newY;

            Repaint();
        }

        BaseNode overNode = null;
        for (int i = 0; i < _mNodes.Count; i++)
        {
            _mNodes[i].CheckMouse(Event.current, graphPan);
            if (_mNodes[i].OverNode)
                overNode = _mNodes[i];
        }

        var prevSel = _selectedNode;
        if (currentE.button == 0 && currentE.type == EventType.MouseDown)
        {
            if (overNode != null)
                _selectedNode = overNode;
            else
                _selectedNode = null;

            if (prevSel != _selectedNode)
                Repaint();
        }
    }

    /// <summary>
    /// Dibuja la barra de herramientas.
    /// </summary>
    void DrawToolbar()
    {
        EditorGUILayout.BeginVertical(GUILayout.Height(100));
        {
            EditorGUILayout.BeginHorizontal();
            {
                _mName = EditorGUILayout.TextField("Map Name", _mName);
                if (GUILayout.Button("Save Map"))
                    SaveMap();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                op = (OPTIONS)EditorGUILayout.EnumPopup("Primitive to create:", op);
                _nodeName = EditorGUILayout.TextField("Node Name", _nodeName);
                if (GUILayout.Button("Add", GUILayout.Width(50), GUILayout.Height(25)))
                {
                    if (_nodeName != "")
                        AddNode();
                    else
                        EditorUtility.DisplayDialog("Error", "You should name your node.", "Ok. I'm Sorry.");
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

        }
        EditorGUILayout.EndVertical();

    }

    /// <summary>
    /// Salva el mapa de nodos.
    /// </summary>
    void SaveMap()
    {

    }

    /// <summary>
    /// Agrega un nuevo nodo.
    /// </summary>
    private void AddNode()
    {
        _mNodes.Add(new BaseNode(0, 0, 200, 150, (int)op, _nodeName));
        _nodeName = "";
        Repaint();
    }

    /// <summary>
    /// Dibuja el espacio nde trabajo.
    /// </summary>
    void DrawWorkSpace()
    {
        graphRect.x = graphPan.x;
        graphRect.y = graphPan.y;
        EditorGUI.DrawRect(new Rect(0, toolbarHeight, position.width, position.height - toolbarHeight), Color.gray);

        GUI.BeginGroup(graphRect);
        {
            BeginWindows();
            {
                var defaultColor = GUI.backgroundColor;

                for (int i = 0; i < _mNodes.Count; i++)
                {
                    foreach (var n in _mNodes[i].connected)
                    {
                        _mNodes[i].myRect = GUI.Window(i, _mNodes[i].myRect, DrawNode, _mNodes[i].name);
                        if (_mNodes[i] == _selectedNode)
                            GUI.backgroundColor = Color.grey;
                        GUI.backgroundColor = defaultColor;

                        // REVISAR DIBUJO DE LINEAS CONECTORAS
                        Handles.DrawLine(new Vector2(_mNodes[i].myRect.position.x + _mNodes[i].myRect.width / 2f,
                            _mNodes[i].myRect.position.y + _mNodes[i].myRect.height / 2f),
                            new Vector2(n.myRect.position.x + n.myRect.width / 2f,
                            n.myRect.position.y + n.myRect.height / 2f));
                    }
                }
            }
            EndWindows();
        }
        GUI.EndGroup();
    }

    /// <summary>
    /// Dibuja el nodo.
    /// </summary>
    /// <param name="id">ID del nodo a dibujar</param>
    private void DrawNode(int id)
    {
        toLink = EditorGUILayout.TextField("Link", toLink);
        if (_mNodes[id].type != 0)
        {
            if ((_mNodes[id].type == 1 && _mNodes[id].connected.Count <= 2))
            {
                if (GUILayout.Button("Connect", GUILayout.Width(50), GUILayout.Height(25)))
                {
                    ConnectNode(id);
                }
            }
        }
        if (GUILayout.Button("Delete", GUILayout.Width(50), GUILayout.Height(25)))
        {
            RemoveNode(id);
        }

        // REVISAR PANEO
        if (!_panningScreen)
        {
            GUI.DragWindow();

            if (!_mNodes[id].OverNode) return;

            if (_mNodes[id].myRect.x < 0)
                _mNodes[id].myRect.x = 0;

            if (_mNodes[id].myRect.y < toolbarHeight - graphPan.y)
                _mNodes[id].myRect.y = toolbarHeight - graphPan.y;
        }
    }

    /// <summary>
    /// Conecta el nodo con el indicado en el campo.
    /// </summary>
    /// <param name="id">ID del nodo a eliminar</param>
    void ConnectNode(int id)
    {
        for (int i = 0; i < _mNodes.Count; i++)
        {
            if (_mNodes[i].name == toLink)
            {
                _mNodes[id].connected.Add(_mNodes[i]);
                toLink = "";
            }
        }

        Repaint();
    }

    /// <summary>
    /// Elimina un nodo y lo saca de todas las listas.
    /// </summary>
    /// <param name="id">ID del nodo a eliminar</param>
    void RemoveNode(int id)
    {
        for (int i = 0; i < _mNodes.Count; i++)
        {
            _mNodes[i].connected.Remove(_mNodes[id]);
        }
        _mNodes.RemoveAt(id);

        Repaint();
    }
}


