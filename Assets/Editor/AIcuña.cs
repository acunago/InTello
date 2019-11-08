using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class AIcuña : EditorWindow
{
    private Rect _toolbarPanel;
    private Rect _editorPanel;

    private float _toolbarHeight = 20f;

    private string _mapName = "Create/Open an AIcuña Map";
    private AIcuñaMap _currentMap;
    private List<Node> _nodes;
    private List<Connection> _connections;

    private GUIStyle _nodeStyle;
    private GUIStyle _selectedNodeStyle;
    private GUIStyle _inPointStyle;
    private GUIStyle _truePointStyle;
    private GUIStyle _falsePointStyle;

    private ConnectionPoint _selectedInPoint;
    private ConnectionPoint _selectedOutPoint;

    private Vector2 _offset;
    private Vector2 _drag;


    [MenuItem("InTello/AIcuña")]
    private static void OpenWindow()
    {
        AIcuña window = GetWindow<AIcuña>();
        window.titleContent = new GUIContent("AIcuña");
    }

    private void OnEnable()
    {
        _nodeStyle = new GUIStyle();
        _nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        _nodeStyle.border = new RectOffset(12, 12, 12, 12);
        _nodeStyle.alignment = TextAnchor.MiddleCenter;
        _nodeStyle.fontStyle = FontStyle.Bold;
        _nodeStyle.normal.textColor = Color.black;
        _nodeStyle.wordWrap = true;

        _selectedNodeStyle = new GUIStyle();
        _selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        _selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        _selectedNodeStyle.alignment = TextAnchor.MiddleCenter;
        _selectedNodeStyle.fontStyle = FontStyle.Bold;
        _selectedNodeStyle.normal.textColor = Color.white;
        _selectedNodeStyle.wordWrap = true;

        _inPointStyle = new GUIStyle();
        _inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node4.png") as Texture2D;
        _inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node4 on.png") as Texture2D;
        _inPointStyle.border = new RectOffset(2, -4, 2, 2);

        _truePointStyle = new GUIStyle();
        _truePointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D;
        _truePointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3 on.png") as Texture2D;
        _truePointStyle.border = new RectOffset(-4, 2, 2, 2);

        _falsePointStyle = new GUIStyle();
        _falsePointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node6.png") as Texture2D;
        _falsePointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node6 on.png") as Texture2D;
        _falsePointStyle.border = new RectOffset(-4, 2, 2, 2);
    }

    private void OnGUI()
    {
        DrawToolbarPanel();
        DrawEditorPanel();

        if (GUI.changed) Repaint();
    }

    #region ---------- DRAW SECTION ----------

    private void DrawToolbarPanel()
    {
        _toolbarPanel = new Rect(0, 0, position.width, _toolbarHeight);

        GUILayout.BeginArea(_toolbarPanel, EditorStyles.toolbar);
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent("N", "New Map."),
                    EditorStyles.toolbarButton, GUILayout.Width(_toolbarHeight)))
                    OnClickNewMap();
                if (GUILayout.Button(new GUIContent("O", "Open Map."),
                    EditorStyles.toolbarButton, GUILayout.Width(_toolbarHeight)))
                    OnClickOpenMap(Selection.activeObject);
                _mapName = EditorGUILayout.TextField(_mapName, EditorStyles.toolbarTextField);
                if (GUILayout.Button(new GUIContent("S", "Save Map."),
                    EditorStyles.toolbarButton, GUILayout.Width(_toolbarHeight)))
                    OnClickSaveMap(_currentMap);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent("Q", "New Question Node."),
                    EditorStyles.toolbarButton, GUILayout.Width(_toolbarHeight)))
                    OnClickAddQuestionNode(_editorPanel.position);
                if (GUILayout.Button(new GUIContent("A", "New Action Node."),
                    EditorStyles.toolbarButton, GUILayout.Width(_toolbarHeight)))
                    OnClickAddActionNode(_editorPanel.position);
                if (GUILayout.Button(new GUIContent("X", "Delete Selected Node."),
                    EditorStyles.toolbarButton, GUILayout.Width(_toolbarHeight)))
                    OnClickRemoveSelectedNode();
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    private void DrawEditorPanel()
    {
        _editorPanel = new Rect(0, _toolbarHeight, position.width, position.height - _toolbarHeight);

        GUILayout.BeginArea(_editorPanel);
        {
            GUILayout.Label(" - Workspace - ");

            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            DrawNodes();
            DrawConnections();
            DrawConnectionLine(Event.current);

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);
        }
        GUILayout.EndArea();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        {
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            _offset += _drag * 0.5f;
            Vector3 newOffset = new Vector3(_offset.x % gridSpacing, _offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
        }
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        if (_nodes != null)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].Draw();
            }
        }
    }

    private void DrawConnections()
    {
        if (_connections != null)
        {
            for (int i = 0; i < _connections.Count; i++)
            {
                _connections[i].Draw();
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (_selectedInPoint != null && _selectedOutPoint == null)
        {
            Handles.DrawBezier(
                _selectedInPoint.rect.center,
                e.mousePosition,
                _selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (_selectedOutPoint != null && _selectedInPoint == null)
        {
            Handles.DrawBezier(
                _selectedOutPoint.rect.center,
                e.mousePosition,
                _selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    #endregion

    #region ---------- PROCESS SECTION ----------

    private void ProcessEvents(Event e)
    {
        _drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 2)
                {
                    OnDrag(e.delta);
                }
                break;

            case EventType.KeyDown:
                if (e.keyCode == KeyCode.Escape)
                {
                    ClearConnectionSelection();
                }
                if (e.keyCode == KeyCode.Delete)
                {
                    OnClickRemoveSelectedNode();
                }
                break;
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if (_nodes != null)
        {
            for (int i = _nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = _nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Question Node"), false, () => OnClickAddQuestionNode(mousePosition));
        genericMenu.AddItem(new GUIContent("Add Action Node"), false, () => OnClickAddActionNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    #endregion

    #region ---------- METHOD SECTION ----------

    private void OnDrag(Vector2 delta)
    {
        _drag = delta;

        if (_nodes != null)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void OnClickNewMap()
    {
        AIcuñaMap asset = ScriptableObject.CreateInstance<AIcuñaMap>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(AIcuñaMap).ToString() + ".asset");

        Debug.Log(assetPathAndName);
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        Debug.Log(path);
        _mapName = AssetDatabase.GetImplicitAssetBundleName(assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        OnClickSaveMap(asset);

        OnClickOpenMap(asset);
    }

    private void OnClickOpenMap(object asset)
    {
        ActionNode auxAction;
        QuestionNode auxQuestion;
        if (asset.GetType() != typeof(AIcuñaMap))
        {
            EditorUtility.DisplayDialog("Error", "You must select an AIcuña Map.", "Ok. I'm Sorry.");
            return;
        }

        _currentMap = (AIcuñaMap)asset;

        if (_currentMap.actions == null) _currentMap.actions = new List<ActionNode>();
        if (_currentMap.questions == null) _currentMap.questions = new List<QuestionNode>();
        if (_currentMap.connections == null) _currentMap.connections = new List<Connection>();

        _mapName = _currentMap.name;

        _nodes = new List<Node>();
        _connections = new List<Connection>();

        foreach (var item in _currentMap.actions)
        {
            auxAction = new ActionNode(item.rect.position, item.rect.width, item.rect.height,
                _nodeStyle, _selectedNodeStyle, _inPointStyle, OnClickInPoint, OnClickRemoveNode,
                item.id, item.inPoint.id);
            auxAction.goName = item.goName;
            auxAction.scriptName = item.scriptName;
            auxAction.methodName = item.methodName;
            auxAction.SelectScript();
            auxAction.SelectMethod();
            _nodes.Add(auxAction);


        }
        foreach (var item in _currentMap.questions)
        {
            auxQuestion = new QuestionNode(item.rect.position, item.rect.width, item.rect.height,
            _nodeStyle, _selectedNodeStyle, _inPointStyle, _truePointStyle, _falsePointStyle,
            OnClickInPoint, OnClickOutPoint, OnClickRemoveNode,
            item.id, item.inPoint.id, item.truePoint.id, item.falsePoint.id);

            auxQuestion.goName = item.goName;
            auxQuestion.scriptName = item.scriptName;
            auxQuestion.methodName = item.methodName;
            auxQuestion.SelectScript();
            auxQuestion.SelectMethod();
            _nodes.Add(auxQuestion);
        }

        foreach (var item in _currentMap.connections)
        {
            ConnectionPoint inPoint = null;
            ConnectionPoint outPoint = null;

            foreach (var node in _nodes)
            {
                if (node.GetType() == typeof(ActionNode))
                {
                    ActionNode n = node as ActionNode;
                    if (item.inPoint.id == n.inPoint.id)
                        inPoint = n.inPoint;
                }
                else if (node.GetType() == typeof(QuestionNode))
                {
                    QuestionNode n = node as QuestionNode;
                    if (item.inPoint.id == n.inPoint.id)
                        inPoint = n.inPoint;
                    if (item.outPoint.id == n.truePoint.id)
                        outPoint = n.truePoint;
                    else if (item.outPoint.id == n.falsePoint.id)
                        outPoint = n.falsePoint;
                }
                if (inPoint != null && outPoint != null) break;
            }

            if (inPoint != null && outPoint != null)
                _connections.Add(new Connection(inPoint, outPoint, OnClickRemoveConnection));
            else
                EditorUtility.DisplayDialog("Error", "Something go wrong.", "Ok. This is a stupid msg.");
        }

        EditorUtility.DisplayDialog("Success Open", "AIcuña map has been opened correctly.", "Ok. Let me work.");
    }

    private void OnClickSaveMap(AIcuñaMap map)
    {
        if (map == null)
        {
            EditorUtility.DisplayDialog("Error", "Something go wrong.", "Ok. This is a stupid msg.");
            return;
        }

        if (_nodes == null)
        {
            _nodes = new List<Node>();
        }
        if (_connections == null)
        {
            _connections = new List<Connection>();
        }

        string assetPath = AssetDatabase.GetAssetPath(map.GetInstanceID());
        AssetDatabase.RenameAsset(assetPath, _mapName);
        AssetDatabase.SaveAssets();

        map.actions = new List<ActionNode>();
        map.questions = new List<QuestionNode>();
        map.connections = new List<Connection>(_connections);

        foreach (var item in _nodes)
        {
            if (item.GetType() == typeof(ActionNode))
                map.actions.Add(item as ActionNode);
            else if (item.GetType() == typeof(QuestionNode))
                map.questions.Add(item as QuestionNode);
        }

        EditorUtility.DisplayDialog("Success Save", "AIcuña map has been saved correctly.", "Ok. You're awesome.");
    }

    private void OnClickAddQuestionNode(Vector2 position)
    {
        if (_nodes == null)
        {
            _nodes = new List<Node>();
        }

        _nodes.Add(new QuestionNode(position, 200, 60,
            _nodeStyle, _selectedNodeStyle, _inPointStyle, _truePointStyle, _falsePointStyle,
            OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    private void OnClickAddActionNode(Vector2 position)
    {
        if (_nodes == null)
        {
            _nodes = new List<Node>();
        }

        _nodes.Add(new ActionNode(position, 200, 50,
            _nodeStyle, _selectedNodeStyle, _inPointStyle,
            OnClickInPoint, OnClickRemoveNode));
    }

    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        _selectedInPoint = inPoint;

        if (_selectedOutPoint != null)
        {
            if (_selectedOutPoint.node != _selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        _selectedOutPoint = outPoint;

        if (_selectedInPoint != null)
        {
            if (_selectedOutPoint.node != _selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        _connections.Remove(connection);
    }

    private void OnClickRemoveNode(Node node)
    {
        if (_connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < _connections.Count; i++)
            {
                if (_connections[i].inPoint.node == node || _connections[i].outPoint.node == node)
                {
                    connectionsToRemove.Add(_connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                _connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        _nodes.Remove(node);
    }

    private void OnClickRemoveSelectedNode()
    {
        if (_nodes != null)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].isSelected)
                {
                    _nodes[i].OnRemoveNode(_nodes[i]);
                }
            }
        }

        GUI.changed = true;
    }

    private void CreateConnection()
    {
        if (_connections == null)
        {
            _connections = new List<Connection>();
        }

        for (int i = 0; i < _connections.Count; i++)
        {
            if (_connections[i].outPoint == _selectedOutPoint)
            {
                _connections.RemoveAt(i);
            }
        }

        _connections.Add(new Connection(_selectedInPoint, _selectedOutPoint, OnClickRemoveConnection));
    }

    private void ClearConnectionSelection()
    {
        _selectedInPoint = null;
        _selectedOutPoint = null;
    }

    #endregion
}
