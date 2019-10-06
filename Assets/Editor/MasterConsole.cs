using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MasterConsole : EditorWindow
{
    int mDisplay;    // Indica que pantalla se muestra (0: root)

    [MenuItem("InTello/Master Console")]
    public static void OpenWindow()
    {
        GetWindowWithRect(typeof(MasterConsole), new Rect(0, 0, 140, 160), true).Show();
    }

    private void OnEnable()
    {
        mDisplay = 0;
    }

    void OnGUI()
    {

        if (mDisplay == 0) DrawConsole();
        else DrawDynamics(mDisplay);

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
    /// Crea la visualizacion de la botonera principal
    /// </summary>
    void DrawConsole()
    {
        if (GUILayout.Button(new GUIContent("WPS", "WayPoints Systems.")))
            mDisplay = 1;
        Repaint();
    }

    void DrawDynamics(int i)
    {
        if (mDisplay == 1)
        {
            WayPointSystemEW.OpenWindow();
            mDisplay = 0;
        }
    }
}
