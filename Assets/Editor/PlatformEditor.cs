using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlatformEditor : EditorWindow
{
    private GUIStyle myStyle;
    private GameObject _gameObj;
    private PlatformScript plat;
    private List<GameObject> listObj = new List<GameObject>();
    private GameObject objItem;
    [MenuItem("InTello/PlatformBehaviour")]
    public static void OpenWindow()
    {


        var mySelf = GetWindow<PlatformEditor>();
        //mySelf.allNodes = new List<BaseNode>();
        mySelf.myStyle = new GUIStyle();
        mySelf.myStyle.fontSize = 20;
        mySelf.myStyle.alignment = TextAnchor.MiddleCenter;
        mySelf.myStyle.fontStyle = FontStyle.BoldAndItalic;


    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.Height(100));
        EditorGUILayout.LabelField("Door Behaviour", myStyle, GUILayout.Height(50));
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        EditorGUIUtility.labelWidth = 50;
        EditorGUIUtility.fieldWidth = 70;
        EditorGUILayout.LabelField("Platform Object", GUILayout.Height(50));
        _gameObj = (GameObject)EditorGUILayout.ObjectField(_gameObj, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        if (_gameObj != null)
        {
            if (GUILayout.Button("Add", GUILayout.Width(50), GUILayout.Height(25)))
            {
                if (_gameObj.AddComponent<PlatformScript>() == null)
                {
                    _gameObj.AddComponent<PlatformScript>();
                }

            }

            plat = _gameObj.GetComponent<PlatformScript>();
            if (plat != null)
            {
                if (_gameObj.GetComponent<BoxCollider>() == null)
                {
                    _gameObj.AddComponent<BoxCollider>();
                }
                plat.objectsToChange = EditorGUILayout.FloatField("Num Objects to change", plat.objectsToChange);

                EditorGUILayout.LabelField("Items to active/Disable", GUILayout.Height(50));
                EditorGUILayout.BeginHorizontal();
                objItem = (GameObject)EditorGUILayout.ObjectField(objItem, typeof(GameObject), true);
                if (GUILayout.Button("Add", GUILayout.Width(50), GUILayout.Height(25)))
                {
                    listObj.Add(objItem);
                }
                EditorGUILayout.EndHorizontal();
                plat.interact = listObj;
            }
        }
    }
}
