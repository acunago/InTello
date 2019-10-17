using System.Collections;
using System.Collections.Generic;

public class ActionDT : IDecision
{
    public delegate void Action();
    Action _action;

    public ActionDT(Action action)
    {
        _action = action;
    }

    public void Execute()
    {
        _action();
    }
}
