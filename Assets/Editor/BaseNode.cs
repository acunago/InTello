using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNode
{
    public Rect myRect;

    public string name;
    public int type;

    public float Id;
    public bool start;
    private bool _overNode;
    public List<BaseNode> connected;

    public BaseNode(float x, float y, float width, float height, int _type,string _name)
    {
        myRect = new Rect(x, y, width, height);
        connected = new List<BaseNode>();
        type = _type;
        name = _name;
    }

    public void CheckMouse(Event cE, Vector2 pan)
    {
        if (myRect.Contains(cE.mousePosition - pan))
            _overNode = true;
        else
            _overNode = false;
    }

    public bool OverNode
    { get { return _overNode; } }
}
