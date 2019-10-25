using System;
using System.Collections;
using System.Collections.Generic;

public class QuestionDT : IDecision
{
    private Func<bool> _question;
    private IDecision _trueNode;
    private IDecision _falseNode;

    public QuestionDT(Func<bool> myQuestion, IDecision trueNode, IDecision falseNode)
    {
        _question = myQuestion;
        _trueNode = trueNode;
        _falseNode = falseNode;
    }

    public void Execute()
    {
        if (_question())
            _trueNode.Execute();
        else
            _falseNode.Execute();
    }
}
