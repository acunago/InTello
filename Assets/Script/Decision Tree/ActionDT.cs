using System;
using System.Collections;
using System.Collections.Generic;

public class ActionDT : IDecision
{
    private Action _action;

    public ActionDT(Action action)
    {
        _action = action;
    }

    public void Execute()
    {
        _action();
    }
}
