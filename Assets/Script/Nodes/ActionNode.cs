using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : BaseNode
{
    // ACA VA LO DEL EVENT

    /// <summary>
    /// Constructor de nodo action.
    /// </summary>
    /// <param name="x">Rect X</param>
    /// <param name="y">Rect Y</param>
    /// <param name="w">Rect Width</param>
    /// <param name="h">Rect Height</param>
    /// <param name="n">Node Name</param>
    public ActionNode(float x, float y, float w, float h, string n) : base(x, y, w, h, n)
    {
        inputs = 1;
        outputs = 0;
    }


}
