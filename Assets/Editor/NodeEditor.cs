using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeEditor : EditorWindow
{
    private Rect toolbarPanel;
    private Rect editorPanel;

    private float toolbarHeight = 20f;

    private string mapName = "Create/Open an AIcuña Map";
    private AIcuñaMap currentMap;
    private List<Node> nodes;
    private List<Connection> connections;

    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle truePointStyle;
    private GUIStyle falsePointStyle;

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    private Vector2 offset;
    private Vector2 drag;

    [MenuItem("InTello/Test Node Editor")]
    private static void OpenWindow()
    {
        NodeEditor window = GetWindow<NodeEditor>();
        window.titleContent = new GUIContent("Node Editor");
    }

    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        nodeStyle.alignment = TextAnchor.MiddleCenter;
        nodeStyle.fontStyle = FontStyle.Bold;
        nodeStyle.normal.textColor = Color.black;
        nodeStyle.wordWrap = true;

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        selectedNodeStyle.alignment = TextAnchor.MiddleCenter;
        selectedNodeStyle.fontStyle = FontStyle.Bold;
        selectedNodeStyle.normal.textColor = Color.white;
        selectedNodeStyle.wordWrap = true;

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        truePointStyle = new GUIStyle();
        truePointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        truePointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        truePointStyle.border = new RectOffset(4, 4, 12, 12);

        falsePointStyle = new GUIStyle();
        falsePointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        falsePointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        falsePointStyle.border = new RectOffset(4, 4, 12, 12);
    }

    private void OnGUI()
    {
        DrawToolbarPanel();
        DrawEditorPanel();

        if (GUI.changed) Repaint();
    }

    #region DRAWs

    private void DrawToolbarPanel()
    {
        toolbarPanel = new Rect(0, 0, position.width, toolbarHeight);

        GUILayout.BeginArea(toolbarPanel, EditorStyles.toolbar);
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent("N", "New Map."),
                    EditorStyles.toolbarButton, GUILayout.Width(toolbarHeight)))
                    OnClickNewMap();
                if (GUILayout.Button(new GUIContent("O", "Open Map."),
                    EditorStyles.toolbarButton, GUILayout.Width(toolbarHeight)))
                    OnClickOpenMap(Selection.activeObject);
                mapName = EditorGUILayout.TextField(mapName, EditorStyles.toolbarTextField);
                if (GUILayout.Button(new GUIContent("S", "Save Map."),
                    EditorStyles.toolbarButton, GUILayout.Width(toolbarHeight)))
                    OnClickSaveMap(currentMap);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent("Q", "New Question Node."),
                    EditorStyles.toolbarButton, GUILayout.Width(toolbarHeight)))
                    OnClickAddQuestionNode(editorPanel.position);
                if (GUILayout.Button(new GUIContent("A", "New Action Node."),
                    EditorStyles.toolbarButton, GUILayout.Width(toolbarHeight)))
                    OnClickAddActionNode(editorPanel.position);
                if (GUILayout.Button(new GUIContent("X", "Delete Selected Node."),
                    EditorStyles.toolbarButton, GUILayout.Width(toolbarHeight)))
                {
                    if (nodes != null)
                    {
                        for (int i = 0; i < nodes.Count; i++)
                        {
                            if(nodes[i].isSelected)
                                nodes[i].OnRemoveNode(nodes[i]);
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    private void DrawEditorPanel()
    {
        editorPanel = new Rect(0, toolbarHeight, position.width, position.height - toolbarHeight);

        GUILayout.BeginArea(editorPanel);
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

            offset += drag * 0.5f;
            Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

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
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    #endregion

    #region PROCESS

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

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
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

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

    #region METHODs

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

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        OnClickSaveMap(asset);

        OnClickOpenMap(asset);
    }

    private void OnClickOpenMap(object asset)
    {
        if (asset.GetType() != typeof(AIcuñaMap))
        {
            EditorUtility.DisplayDialog("Error", "You must select an AIcuña Map.", "Ok. I'm Sorry.");
            return;
        }

        currentMap = (AIcuñaMap)asset;

        if (currentMap.nodes == null)
        {
            currentMap.nodes = new List<Node>();
        }
        if (currentMap.connections == null)
        {
            currentMap.connections = new List<Connection>();
        }

        mapName = currentMap.name;

        nodes = new List<Node>(currentMap.nodes);
        connections = new List<Connection>(currentMap.connections);

        EditorUtility.DisplayDialog("Success Open", "AIcuña map has been opened correctly.", "Ok. Let me work.");
    }

    private void OnClickSaveMap(AIcuñaMap map)
    {
        if (map == null)
        {
            EditorUtility.DisplayDialog("Error", "Something go wrong.", "Ok. This is a stupid msg.");
            return;
        }

        if (nodes == null)
        {
            nodes = new List<Node>();
        }
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        map.name = mapName;
        map.nodes = new List<Node>(nodes);
        map.connections = new List<Connection>(connections);

        EditorUtility.DisplayDialog("Success Save", "AIcuña map has been saved correctly.", "Ok. You're awesome.");
    }

    /// <summary>
    /// Drag del mapa.
    /// </summary>
    /// <param name="delta"></param>
    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    /// <summary>
    /// Crea un nodo de pregunta.
    /// </summary>
    /// <param name="position">Posicion del nodo.</param>
    private void OnClickAddQuestionNode(Vector2 position)
    {
        if (nodes == null)
        {
            nodes = new List<Node>();
        }

        nodes.Add(new QuestionNode(position, 200, 60,
            nodeStyle, selectedNodeStyle, inPointStyle, truePointStyle, falsePointStyle,
            OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    /// <summary>
    /// Crea un nodo de accion.
    /// </summary>
    /// <param name="position">Posicion del nodo.</param>
    private void OnClickAddActionNode(Vector2 position)
    {
        if (nodes == null)
        {
            nodes = new List<Node>();
        }

        nodes.Add(new ActionNode(position, 200, 50,
            nodeStyle, selectedNodeStyle, inPointStyle,
            OnClickInPoint, OnClickRemoveNode));
    }

    /// <summary>
    /// Conecta un punto de entrada.
    /// </summary>
    /// <param name="inPoint">Punto de entrada a conectar.</param>
    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
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

    /// <summary>
    /// Conecta un punto de salida.
    /// </summary>
    /// <param name="outPoint">Punto de salida a conectar.</param>
    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
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

    /// <summary>
    /// Elimina la conexion indicada.
    /// </summary>
    /// <param name="connection">Conexion a eliminar.</param>
    private void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }

    /// <summary>
    /// Elimina el nodo indicado.
    /// </summary>
    /// <param name="node">Nodo a eliminar.</param>
    private void OnClickRemoveNode(Node node)
    {
        if (connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint.node == node || connections[i].outPoint.node == node)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        nodes.Remove(node);
    }

    /// <summary>
    /// Crea una conexion entre nodos.
    /// </summary>
    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    /// <summary>
    /// Reinicia la conexion en curso.
    /// </summary>
    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    #endregion
}
