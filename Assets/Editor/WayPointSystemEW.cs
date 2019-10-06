using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class WayPointSystemEW : EditorWindow
{
    List<Transform> _WPSs;          // Lista de los WPS
    int _currentWPS;                // Indice del WPS actual
    List<Transform> _waypoints;     // Puntos del WPS actual

    Vector2 _scrollPosition;

    [MenuItem("InTello/WPS: Way Point System")]
    public static void OpenWindow()
    {
        GetWindowWithRect(typeof(WayPointSystemEW), new Rect(0, 0, 140, 160), true).Show();
    }

    private void OnEnable()
    {
        FindWPS();
        _currentWPS = 0;
        UpdateWayPoints();
    }

    void OnGUI()
    {
        DrawCommands();

        if (GUI.changed)
        {
            SceneView.RepaintAll();
            Repaint();
        }
    }

    /// <summary>
    /// Crea una linea de separacion entre conjuntos de elementos
    /// </summary>
    void Separator()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }

    /// <summary>
    /// Crea la interfaz de visualizacion de la herramienta
    /// </summary>
    public void DrawCommands()
    {
        var defaultColor = GUI.backgroundColor;

        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent("New WPS", "Creates a new Way Point System.")))
                    NewWPS();
                if (GUILayout.Button(new GUIContent("Del WPS", "Delete the selected Way Point System.")))
                    DeleteWPS();
            }
            EditorGUILayout.EndHorizontal();
            _currentWPS = EditorGUILayout.Popup(new GUIContent("", "Select a Way Point System."), _currentWPS, WPSNames());
            Separator();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent("O", "Set as starting point.")))
                    MakeStart();
                if (GUILayout.Button(new GUIContent("+", "Add point.")))
                    AddPoint();
                if (GUILayout.Button(new GUIContent("-", "Remove point.")))
                    RemovePoint();
            }
            EditorGUILayout.EndHorizontal();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                var style = new GUIStyle(GUI.skin.button);
                style.alignment = TextAnchor.MiddleLeft;
                style.normal.background = Texture2D.whiteTexture;
                style.normal.textColor = Color.black;
                style.active.background = Texture2D.blackTexture;
                style.active.textColor = Color.white;
                style.focused.background = Texture2D.blackTexture;
                style.focused.textColor = Color.white;

                for (int i = 0; i < _waypoints.Count; i++)
                {
                    if (_waypoints[i] == Selection.activeTransform)
                        GUI.backgroundColor = Color.yellow;
                    else
                        GUI.backgroundColor = Color.white;

                    if (GUILayout.Button(_waypoints[i].name, style))
                    {
                        Selection.activeTransform = _waypoints[i];
                        Repaint();
                    }
                }

                GUI.backgroundColor = defaultColor;
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Genera un array con los nombres de la lista de WPS
    /// </summary>
    /// <returns></returns>
    string[] WPSNames()
    {
        if (_WPSs == null || _WPSs.Count == 0)
            return new string[] { "No WPSs" };
        else
        {
            string[] names = new string[_WPSs.Count];
            for (int i = 0; i < _WPSs.Count; i++)
            {
                if (_WPSs[i] == null)
                    names[i] = "Empty";
                else
                    names[i] = _WPSs[i].name;
            }
            return names;
        }
    }

    /// <summary>
    /// Busca los WPS existentes
    /// </summary>
    void FindWPS()
    {
        _WPSs = new List<Transform>();
        Transform[] myScene = GameObject.FindObjectsOfType<Transform>();
        if (myScene == null) return;
        for (int i = 0; i < myScene.Length; i++)
        {
            if (myScene[i].name.StartsWith("WPS:"))
                _WPSs.Add(myScene[i]);
        }
    }

    /// <summary>
    /// Busca los puntos existentes en el sistema
    /// </summary>
    void UpdateWayPoints()
    {
        _waypoints = new List<Transform>();
        if (_WPSs.Count <= _currentWPS) return;
        Transform[] childs = _WPSs[_currentWPS].GetComponentsInChildren<Transform>();
        if (childs == null) return;
        for (int i = 0; i < childs.Length; i++)
        {
            if (childs[i].GetComponent<WayPoint>() != null)
            {
                if (childs[i].GetComponent<WayPoint>().GetPrevious() != null)
                    _waypoints.Add(childs[i]);
                else
                {
                    if (childs[0].GetComponent<WayPoint>().GetPrevious() != null)
                        _waypoints.Insert(0, childs[i]);
                    else
                        Debug.Log("More than one Start Point");
                }


            }

        }
    }

    /// <summary>
    /// Crea un nuevo WPS
    /// </summary>
    void NewWPS()
    {
        var newWPS = new GameObject("WPS: Name" + (_WPSs.Count + 1));
        Undo.RegisterCreatedObjectUndo(newWPS, "WPS Created");
        _WPSs.Add(newWPS.transform);
        Selection.activeGameObject = newWPS;
        Repaint();
    }

    /// <summary>
    /// Elimina el WPS seleccionado
    /// </summary>
    void DeleteWPS()
    {
        if (_WPSs == null || _WPSs.Count == 0) return;
        Transform toDelete = _WPSs[_currentWPS];
        _WPSs.Remove(toDelete);
        DestroyImmediate(toDelete.gameObject);
    }

    /// <summary>
    /// Crea el punto incial del sistema o convierte al elegido en el inicio
    /// </summary>
    void MakeStart()
    {
        if (_waypoints.Contains(Selection.activeTransform))
        {
            int idx = _waypoints.IndexOf(Selection.activeTransform);
            if (idx == 0) return;
            _waypoints[0].GetComponent<WayPoint>().SetPrevious(GetLastPoint());
            _waypoints[0].GetComponent<WayPoint>().GetPrevious()
                .GetComponent<WayPoint>().SetNext(_waypoints[0]);
            _waypoints[idx].GetComponent<WayPoint>().GetPrevious()
                .GetComponent<WayPoint>().SetNext(null);
            _waypoints[idx].GetComponent<WayPoint>().SetPrevious(null);
            _waypoints.Remove(Selection.activeTransform);
            _waypoints.Insert(0, Selection.activeTransform);
        }
        else
        {
            var newPoint = new GameObject("Point" + (_waypoints.Count + 1));
            newPoint.transform.SetParent(_WPSs[_currentWPS]);
            _waypoints.Insert(0, newPoint.transform);
            newPoint.AddComponent(typeof(WayPoint));
            newPoint.GetComponent<WayPoint>().SetPrevious(null);
            newPoint.GetComponent<WayPoint>().SetNext(_waypoints[1]);
            _waypoints[1].GetComponent<WayPoint>().SetPrevious(newPoint.transform);
            Selection.activeGameObject = newPoint;
            Repaint();
        }
    }

    /// <summary>
    /// Agrega un punto al WPS
    /// </summary>
    void AddPoint()
    {
        var newPoint = new GameObject("Point" + (_waypoints.Count + 1));
        newPoint.transform.SetParent(_WPSs[_currentWPS]);
        _waypoints.Add(newPoint.transform);
        newPoint.AddComponent(typeof(WayPoint));

        if (_waypoints.Contains(Selection.activeTransform))
        {
            newPoint.GetComponent<WayPoint>().SetPrevious(Selection.activeTransform);
            newPoint.GetComponent<WayPoint>().SetNext(Selection.activeTransform.GetComponent<WayPoint>().GetNext());
            Selection.activeTransform.GetComponent<WayPoint>().SetNext(newPoint.transform);
            newPoint.GetComponent<WayPoint>().GetNext().GetComponent<WayPoint>().SetPrevious(newPoint.transform);
        }
        else
        {
            newPoint.GetComponent<WayPoint>().SetPrevious(GetLastPoint());
            newPoint.GetComponent<WayPoint>().GetPrevious()
                .GetComponent<WayPoint>().SetNext(newPoint.transform);
            newPoint.GetComponent<WayPoint>().SetNext(null);
        }
        Selection.activeGameObject = newPoint;
        Repaint();
    }

    /// <summary>
    /// Delete a point in the WPS
    /// </summary>
    void RemovePoint()
    {
        if (!Selection.activeTransform) return;

        if (_waypoints.Contains(Selection.activeTransform))
        {
            WayPoint wp = Selection.activeTransform.GetComponent<WayPoint>();
            if (wp.GetPrevious() != null)
            {
                if (wp.GetNext() != null)
                {
                    wp.GetNext().GetComponent<WayPoint>().SetPrevious(wp.GetPrevious());
                    wp.GetPrevious().GetComponent<WayPoint>().SetNext(wp.GetNext());
                }
                else
                {
                    wp.GetPrevious().GetComponent<WayPoint>().SetNext(null);
                }
            }
            else
            {
                if (wp.GetNext() != null)
                {
                    wp.GetNext().GetComponent<WayPoint>().SetPrevious(null);
                    _waypoints.Remove(wp.GetNext());
                    _waypoints.Insert(0, wp.GetNext());
                }
            }
            _waypoints.Remove(Selection.activeTransform);
            DestroyImmediate(Selection.activeTransform.gameObject);
        }
        else
        {
            Debug.Log("You must select a point included on the WPS");
        }

    }

    /// <summary>
    /// Obtiene el transform del ultimo punto del WPS
    /// </summary>
    /// <returns></returns>
    Transform GetLastPoint()
    {
        Transform tr = null;
        if (_waypoints.Count > 0 || _waypoints[0] != null)
            for (int i = 0; i < _waypoints.Count; i++)
            {
                if (_waypoints[i].GetComponent<WayPoint>().GetNext() == null)
                    tr = _waypoints[i].GetComponent<WayPoint>().GetNext();
            }
        return tr;
    }
}
