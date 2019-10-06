using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

using System;
using System.IO;


public enum OPTIONS
{
    ACTION = 0,
    QUESTION = 1,
    TREE = 2
}




public class NodeEditor : EditorWindow
{

    private List<BaseNode> allNodes = new List<BaseNode>();
    private List<NodeTranslate> allNodesTrans = new List<NodeTranslate>();
    private NodeTranslateCollection nodeCol ;
    private GUIStyle myStyle;
    private INode nodeAux;
    private GameObject levelAux;
    private float toolbarHeight = 100;

    private BaseNode _selectedNode;

    //para el paneo
    private bool _panningScreen;
    private Vector2 graphPan;
    private Vector2 _originalMousePosition;
    private Vector2 prevPan;
    private Rect graphRect;

    public GUIStyle wrapTextFieldStyle;
    public OPTIONS op;
    //LevelManager LvlMan;

    GameObject go;

    private string name = "";
    private string n = "";

    public string data;
    public string path;

    [MenuItem("CustomTools/MyNodeEditor")]
    //[CustomEditor(typeof(LevelManager)), CanEditMultipleObjects]
    public static void OpenWindow()
    {
        

        var mySelf = GetWindow<NodeEditor>();
        //mySelf.allNodes = new List<BaseNode>();
        mySelf.myStyle = new GUIStyle();
        mySelf.myStyle.fontSize = 20;
        mySelf.myStyle.alignment = TextAnchor.MiddleCenter;
        mySelf.myStyle.fontStyle = FontStyle.BoldAndItalic;

        mySelf.graphPan = new Vector2(0, mySelf.toolbarHeight);
        mySelf.graphRect = new Rect(0, mySelf.toolbarHeight, 1000000, 1000000);

        //creo un style para asignar a los textos de manera que usen wordwrap
        //le paso el style por defecto como parametro para mantener el mismo "look"
        mySelf.wrapTextFieldStyle = new GUIStyle(EditorStyles.textField);
        mySelf.wrapTextFieldStyle.wordWrap = true;

    }

    private void OnEnable()
    {
        //go = GameObject.Find("GameManager");
        //if (go != null)
        //{
        //    LvlMan = go.GetComponent<LevelManager>();
        //    if (LvlMan == null)
        //    {
        //        LvlMan = go.AddComponent<LevelManager>();
        //    }
        //}
        //else
        //{
        //    go = new GameObject();
        //    go.name = "GameManager";
        //    LvlMan = go.AddComponent<LevelManager>();
        //}

        //if (LvlMan.allNodes != null && LvlMan.allNodes.Count > 0)
        //{
        //    allNodes = LvlMan.allNodes;
        //    Debug.Log("muestro");
        //}
        //serializedObject = new SerializedObject(LvlMan);
        //LoadData();

    }
    private void OnGUI()
    {
        CheckMouseInput(Event.current);
        EditorGUILayout.BeginVertical(GUILayout.Height(100));
        EditorGUILayout.LabelField("Random Levels Editor", myStyle, GUILayout.Height(50));
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        EditorGUIUtility.labelWidth = 30;
        EditorGUIUtility.fieldWidth = 20;
        //nodeAux = (INode)EditorGUILayout.(nodeAux, typeof(INode), true);
        op = (OPTIONS)EditorGUILayout.EnumPopup("Primitive to create:", op);
        name = EditorGUILayout.TextField("Node Name",name);
        if (GUILayout.Button("Add", GUILayout.Width(50), GUILayout.Height(25)))
        {
            if (name != "")
            {
                AddNode();


            }else
            {
                EditorUtility.DisplayDialog("Error", "you should name your node","Ok");
            }
        }


        if (GUILayout.Button("Save", GUILayout.Width(50), GUILayout.Height(25)))
        {
            SaveData();
        }

        EditorGUILayout.Space();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();


        graphRect.x = graphPan.x;
        graphRect.y = graphPan.y;
        EditorGUI.DrawRect(new Rect(0, toolbarHeight, position.width, position.height - toolbarHeight), Color.gray);

        GUI.BeginGroup(graphRect);

        BeginWindows();
        var oriCol = GUI.backgroundColor;
        for (int i = 0; i < allNodes.Count; i++)
        {
            foreach (var c in allNodes[i].connected)
            {
                Handles.DrawLine(new Vector2(allNodes[i].myRect.position.x + allNodes[i].myRect.width / 2f, allNodes[i].myRect.position.y + allNodes[i].myRect.height / 2f), new Vector2(c.myRect.position.x + c.myRect.width / 2f, c.myRect.position.y + c.myRect.height / 2f));
            }
        }

        for (int i = 0; i < allNodes.Count; i++)
        {


            allNodes[i].myRect = GUI.Window(i, allNodes[i].myRect, DrawNode, allNodes[i].name);

            //switch (allNodes[i].type)
            //{
            //    case 0:
            //        GUI.backgroundColor = Color.yellow;
            //        break;
            //    case 1:
            //        GUI.backgroundColor = Color.red;
            //        break;
            //    default:
            //        GUI.backgroundColor = Color.blue;
            //        break;
            //}

            if (allNodes[i] == _selectedNode)
                GUI.backgroundColor = Color.grey;

        }

        EndWindows();
        GUI.EndGroup();
    }


    private void CheckMouseInput(Event currentE)
    {
        if (!graphRect.Contains(currentE.mousePosition) || !(focusedWindow == this || mouseOverWindow == this))
            return;

        //panning
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
        for (int i = 0; i < allNodes.Count; i++)
        {
            allNodes[i].CheckMouse(Event.current, graphPan);
            if (allNodes[i].OverNode)
                overNode = allNodes[i];
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


    private void AddNode()
    {
        allNodes.Add(new BaseNode(0, 0, 200, 150, (int)op, name));
        name = "";
        Repaint();

    }



    private void DrawNode(int id)
    {

        //le dibujamos lo que queramos al nodo...
        EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField("Dialogo", GUILayout.Width(100));

        //allNodes[id].name = EditorGUILayout.TextField(allNodes[id].name, wrapTextFieldStyle, GUILayout.Height(50));
        EditorGUILayout.EndHorizontal();
        //EditorGUILayout.FloatField("Id ", allNodes[id].Id);

        Repaint();

        n = EditorGUILayout.TextField("Link",n);
        if (allNodes[id].type != 0 )
        {
            if((allNodes[id].type == 1 && allNodes[id].connected.Count <= 2)) {
                if (GUILayout.Button("Add", GUILayout.Width(50), GUILayout.Height(25)))
                {
                    Debug.Log(n);

                    for (int i = 0; i < allNodes.Count; i++)
                    {
                        if (allNodes[i].name == n)
                        {
                            Debug.Log("Entra");
                            allNodes[id].connected.Add(allNodes[i]);

                            n = "";

                        }
                    }
                }

            }
        }
        Repaint();
        //if (n != null)
        //{
        //    for (int i = 0; i < allNodes.Count; i++)
        //    {

        //            //if (allNodes[i].Id != 0)
        //            //{
        //            //    level = allNodes[i].level;
        //            //    AddNode();
        //            //}
        //            //allNodes[i].Id = allNodes[id].connected.Count + 1;

        //            //allNodesTrans[id].AddConnect(allNodes[i].level);
                
        //    }

        //}

        if (GUILayout.Button("Delete", GUILayout.Width(50), GUILayout.Height(25)))
        {
            Remove(id);
        }

        Repaint();

        if (!_panningScreen)
        {
            //esto habilita el arrastre del nodo.
            //pasandole como parámetro un Rect podemos setear que la zona "agarrable" a una específica.
            GUI.DragWindow();

            if (!allNodes[id].OverNode) return;

            //clampeamos los valores para asegurarnos que no se puede arrastrar el nodo por fuera del "área" que nosotros podemos panear
            if (allNodes[id].myRect.x < 0)
                allNodes[id].myRect.x = 0;

            if (allNodes[id].myRect.y < toolbarHeight - graphPan.y)
                allNodes[id].myRect.y = toolbarHeight - graphPan.y;
        }
    }

    private void Remove(int id)
    {
        for (int i = 0; i < allNodes.Count; i++)
        {
            allNodes[i].connected.Remove(allNodes[id]);
        }
        allNodes.RemoveAt(id);

    }


    #region setWork
    private void SaveData()
    {

        string auxCon = "";
        allNodesTrans.Clear();
        for (int i = 0; i < allNodes.Count; i++)
        {
            for (int ia = 0; ia < allNodes[i].connected.Count; ia++)
            {
                auxCon = allNodes[i].connected[ia].name+ "," + auxCon;
            }
            NodeTranslate nd = new NodeTranslate(allNodes[i].name, auxCon, allNodes[i].myRect.x, allNodes[i].myRect.y);
            allNodesTrans.Add(nd);
            auxCon = "";
        }


        nodeCol = new NodeTranslateCollection(allNodesTrans);
        data = JsonUtility.ToJson(nodeCol, true);
        
        File.WriteAllText(path, data);
    }

    private void LoadData()
    {

        path = Application.persistentDataPath + "/SavedData.json";

        Debug.Log(path);

        if (File.Exists(path))
        {
            data = File.ReadAllText(path);
            nodeCol = JsonUtility.FromJson<NodeTranslateCollection>(data);
            if (nodeCol != null)
            {
                allNodesTrans = nodeCol.allNodes;
                TranslateNode();
            }
        }
        else
        {
            
            File.WriteAllText(path, "");
        }

    }

    private void TranslateNode()
    {
        for (int i = 0; i < allNodesTrans.Count; i++)
        {
            if (GameObject.Find(allNodesTrans[i].level) != null)
            {
                AddNode(GameObject.Find(allNodesTrans[i].level), allNodesTrans[i].x, allNodesTrans[i].y);

            }

        }

        for (int i = 0; i < allNodes.Count; i++)
        {

            foreach (var item in allNodesTrans[i]._connect.Split(','))
            {
                if (item != "")
                {
                    allNodes[i].connected.Add(allNodes.Find(x => x.name == item.ToString()));

                }
            }
        }
        Repaint();
    }

    private void AddNode(GameObject lvl, float x, float y)
    {

        //allNodes.Add(new BaseNode(x, y, 200, 150, lvl));


        //Repaint();

    }
    #endregion

}
public class NodeTranslateCollection
{
    public List<NodeTranslate> allNodes;

    public NodeTranslateCollection(List<NodeTranslate> node)
    {
        allNodes = node;
    }
}

[Serializable]
public class NodeTranslate
{
    public float x;
    public float y;
    public string level;
    public string _connect;

    public NodeTranslate(string _level, string connect, float _x, float _y)
    {
        x = _x;
        y = _y;
        level = _level;
        _connect = connect;
    }


}
