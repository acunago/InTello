using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DoorEditor : EditorWindow
{
    private GUIStyle myStyle;
    private GameObject _gameObj;
    [MenuItem("InTello/DoorBehaviour")]
    public static void OpenWindow()
    {


        var mySelf = GetWindow<DoorEditor>();
        //mySelf.allNodes = new List<BaseNode>();
        mySelf.myStyle = new GUIStyle();
        mySelf.myStyle.fontSize = 20;
        mySelf.myStyle.alignment = TextAnchor.MiddleCenter;
        mySelf.myStyle.fontStyle = FontStyle.BoldAndItalic;


    }

    private void OnGUI()
    {
        GameObject go;
        EditorGUILayout.BeginVertical(GUILayout.Height(100));
        EditorGUILayout.LabelField("Door Behaviour", myStyle, GUILayout.Height(50));
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        EditorGUIUtility.labelWidth = 50;
        EditorGUIUtility.fieldWidth = 70;
        EditorGUILayout.LabelField("Door Object",GUILayout.Height(50));
        _gameObj = (GameObject)EditorGUILayout.ObjectField(_gameObj, typeof(GameObject), true);

        if(_gameObj == null) { return; }
        if (GUILayout.Button("Add", GUILayout.Width(50), GUILayout.Height(25)))
        {
            if (_gameObj.transform.parent != null)
            {
                if (_gameObj.transform.parent.GetComponent<DoorScript>() != null) { return; }
                go = _gameObj.transform.parent.gameObject;
            }
            else { 
                go = new GameObject();
                go.transform.position = _gameObj.transform.position;
            }

            go.AddComponent<DoorScript>();
            _gameObj.transform.SetParent(go.transform);

        }
    }
}
