using System.Collections;
using System.Collections.Generic;

public class QuestionNode : INode
{
    public delegate bool Question();
    Question _question;
    INode _trueNode;
    INode _falseNode;

    public QuestionNode(Question myQuestion,INode trueNode,INode falseNode)
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
