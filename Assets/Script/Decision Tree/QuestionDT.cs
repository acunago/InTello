using System.Collections;
using System.Collections.Generic;

public class QuestionDT : IDecision
{
    public delegate bool Question();
    Question _question;
    IDecision _trueNode;
    IDecision _falseNode;

    public QuestionDT(Question myQuestion, IDecision trueNode, IDecision falseNode)
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
