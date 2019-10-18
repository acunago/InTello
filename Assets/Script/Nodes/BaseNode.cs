using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BaseNode
{
    public Rect rect;

    public float id;
    public string name;
    public List<BaseNode> connected;
    public int inputs;
    public int outputs;

    private bool _overNode;

    /// <summary>
    /// Constructor de nodo base.
    /// </summary>
    /// <param name="x">Rect X</param>
    /// <param name="y">Rect Y</param>
    /// <param name="w">Rect Width</param>
    /// <param name="h">Rect Height</param>
    /// <param name="n">Node Name</param>
    public BaseNode(float x, float y, float w, float h, string n)
    {
        rect = new Rect(x, y, w, h);
        name = n;
        connected = new List<BaseNode>();
        inputs = 0;
        outputs = 0;
    }

    public void CheckMouse(Event cE, Vector2 pan)
    {
        if (rect.Contains(cE.mousePosition - pan))
            _overNode = true;
        else
            _overNode = false;
    }

    public bool OverNode
    { get { return _overNode; } }
}
