using System.Collections;
using System.Collections.Generic;

public class ActionNode : INode
{
    public delegate void Action();
    Action _action;

    public ActionNode(Action action)
    {
        _action = action;
    }

    public void Execute()
    {
        _action();
    }
}
