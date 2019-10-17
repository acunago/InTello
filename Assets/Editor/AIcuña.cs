using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

public class AIcuña : EditorWindow
{
    string _mName;
    List<BaseNode> _mNodes;
    string _nodeName;

    BaseNode _selectedNode;

    string toLink = ""; // REVISAR COMO CORREGIR ESTO

    // A REVISAR POR PANEO
    private bool _panningScreen;
    private Vector2 graphPan;
    private Vector2 _originalMousePosition;
    private Vector2 prevPan;
    private Rect graphRect;

    // A REVISAR POR DISEÑO
    private float _tbHeight = 30;
    public GUIStyle wrapTextFieldStyle;

    [MenuItem("InTello/AIcuña")]
    public static void OpenWindow()
    {
        var mySelf = GetWindow<AIcuña>();
        mySelf.graphPan = new Vector2(0, mySelf._tbHeight);
        mySelf.graphRect = new Rect(0, mySelf._tbHeight, 1000000, 1000000);

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

        if (GUI.changed)
        {
            SceneView.RepaintAll();
            Repaint();
        }
    }

    /// <summary>
    /// Crea una linea vertical de separacion entre conjuntos de elementos.
    /// </summary>
    void SeparatorV()
    {
        Rect rect = EditorGUILayout.GetControlRect(false);
        rect.y += 5;
        rect.width = 1;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }

    /// <summary>
    /// Crea los iconos de conexion en la posicion indicada.
    /// </summary>
    /// <param name="x">Posicion X</param>
    /// <param name="y">Posicion Y</param>
    /// <param name="c">Cantidad</param>
    void Connectors(float x, float y, int c)
    {
        float iconW = 10;
        float iconH = 10;
        float gap = 5;
        float frame = 2;

        float totalW = 2 * frame + c * (gap + iconW) - gap;
        float totalH = iconH + 2 * frame;

        Rect totalR = new Rect(x - (totalW / 2), y - (totalH / 2), totalW, totalH);
        EditorGUI.DrawRect(totalR, new Color(0.5f, 0.5f, 0.5f, 1));

        for (int i = 0; i < c; i++)
        {
            Rect iconR = new Rect(x - (totalW / 2) + frame + (i) * (gap + iconW), y - (iconH / 2), iconW, iconH);
            EditorGUI.DrawRect(iconR, new Color(0f, 0f, 0f, 1));
        }

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
            graphPan.y = newY > _tbHeight ? _tbHeight : newY;

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
        EditorGUILayout.BeginVertical(GUILayout.Height(_tbHeight));
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent("N", "New Map."),
                    GUILayout.Width(_tbHeight - 5), GUILayout.Height(_tbHeight - 5)))
                    NewMap();
                if (GUILayout.Button(new GUIContent("O", "Open Map."),
                    GUILayout.Width(_tbHeight - 5), GUILayout.Height(_tbHeight - 5)))
                    OpenMap();
                _mName = EditorGUILayout.TextField(_mName, GUILayout.Height(_tbHeight - 10));
                if (GUILayout.Button(new GUIContent("S", "Save Map."),
                    GUILayout.Width(_tbHeight - 5), GUILayout.Height(_tbHeight - 5)))
                    SaveMap();

                GUILayout.Space(3);
                //SeparatorV();

                if (GUILayout.Button(new GUIContent("Q", "New Question Node."),
                    GUILayout.Width(_tbHeight - 5), GUILayout.Height(_tbHeight - 5)))
                    NewQuestion();
                if (GUILayout.Button(new GUIContent("A", "New Action Node."),
                    GUILayout.Width(_tbHeight - 5), GUILayout.Height(_tbHeight - 5)))
                    NewAction();
                //if (GUILayout.Button(new GUIContent("X", "Delete Selected Node.", 
                //    GUILayout.Width(_tbHeight - 5), GUILayout.Height(_tbHeight - 5)))
                //    DeleteNode();

            }
            EditorGUILayout.EndHorizontal();
            /*
            EditorGUILayout.BeginHorizontal();
            {
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
            */
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Crea un nuevo mapa de nodos.
    /// </summary>
    void NewMap()
    {

    }

    /// <summary>
    /// Abre un mapa de nodos.
    /// </summary>
    void OpenMap()
    {

    }

    /// <summary>
    /// Salva el mapa de nodos.
    /// </summary>
    void SaveMap()
    {

    }

    /// <summary>
    /// Crea un nuevo nodo de pregunta.
    /// </summary>
    private void NewQuestion()
    {
        _mNodes.Add(new QuestionNode(0, 0, 200, 150, "New Question"));
        Repaint();
    }

    /// <summary>
    /// Crea un nuevo nodo de accion.
    /// </summary>
    private void NewAction()
    {
        _mNodes.Add(new ActionNode(0, 0, 200, 150, "New Action"));
        Repaint();
    }

    /// <summary>
    /// Elimina un nodo y lo saca de todas las listas.
    /// </summary>
    /// <param name="id">ID del nodo a eliminar</param>
    void DeleteNode(int id)
    {
        for (int i = 0; i < _mNodes[id].connected.Count; i++)
        {
            _mNodes[id].connected[i].connected.Remove(_mNodes[id]);
        }
        _mNodes.RemoveAt(id);

        Repaint();
    }

    /// <summary>
    /// Dibuja el espacio nde trabajo.
    /// </summary>
    void DrawWorkSpace()
    {
        graphRect.x = graphPan.x;
        graphRect.y = graphPan.y;
        EditorGUI.DrawRect(new Rect(0, _tbHeight, position.width, position.height - _tbHeight), Color.gray);

        GUI.BeginGroup(graphRect);
        {
            BeginWindows();
            {
                var defaultColor = GUI.backgroundColor;

                for (int i = 0; i < _mNodes.Count; i++)
                {
                    _mNodes[i].rect = GUI.Window(i, _mNodes[i].rect, DrawNode, _mNodes[i].name);
                    if (_mNodes[i] == _selectedNode)
                        GUI.backgroundColor = Color.grey;
                    GUI.backgroundColor = defaultColor;

                    /* REVISAR DIBUJO DE LINEAS CONECTORAS
                    foreach (var n in _mNodes[i].connected)
                    {
                        Handles.DrawLine(new Vector2(_mNodes[i].rect.position.x + _mNodes[i].rect.width / 2f,
                            _mNodes[i].rect.position.y + _mNodes[i].rect.height / 2f),
                            new Vector2(n.rect.position.x + n.rect.width / 2f,
                            n.rect.position.y + n.rect.height / 2f));
                    }
                    */
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
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUIUtility.labelWidth = 40;
                _mNodes[id].name = EditorGUILayout.TextField(new GUIContent("Name", "Node Name."), _mNodes[id].name);
                if (GUILayout.Button(new GUIContent("X", "Delete Node."),
                    GUILayout.Width(18), GUILayout.Height(18)))
                    DeleteNode(id);
            }
            EditorGUILayout.EndHorizontal();

            // ACA VA LO DEL UNITYEVENT

            if (_mNodes[id].outputs > 0)
                Connectors(100, 143, _mNodes[id].outputs); // ARREGLAR CON DIMENSIONES DE VENTANA DINAMICA
        }
        EditorGUILayout.EndVertical();

        // REVISAR PANEO
        if (!_panningScreen)
        {
            GUI.DragWindow();

            if (!_mNodes[id].OverNode) return;

            if (_mNodes[id].rect.x < 0)
                _mNodes[id].rect.x = 0;

            if (_mNodes[id].rect.y < _tbHeight - graphPan.y)
                _mNodes[id].rect.y = _tbHeight - graphPan.y;
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
}


