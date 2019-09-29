using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class WayPointSystem : EditorWindow
{
    public static void OpenWindow()
    {
        GetWindow(typeof(WayPointSystem)).Show();
    }

    private void OnEnable()
    {
        
    }

    void OnGUI()
    {
        DrawCommands();
        if (GUILayout.Button(new GUIContent("Close", "Close Window.")))
            Close();
    }

    public void DrawCommands()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent("O", "Set as starting point.")))
                    MakeStart();
                if (GUILayout.Button(new GUIContent("+", "Add point.")))
                    AddPoint();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent("-", "Remove point.")))
                    RemovePoint();
                if (GUILayout.Button(new GUIContent("X", "Delete system.")))
                    DeleteSystem();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    void MakeStart()
    {
        
    }

    void AddPoint()
    {
        
    }

    void RemovePoint()
    {
        
    }

    void DeleteSystem()
    {
        
    }
}
