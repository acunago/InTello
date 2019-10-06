using System.Collections;
using System.Collections.Generic;

public class OptionsNode : INode
{
    public delegate int Option();
    Option _option;
    List<INode> _optionsNodes;

    public OptionsNode(Option myOption, List<INode> myOptionsNodes)
    {
        _option = myOption;
        _optionsNodes = myOptionsNodes;
    }

    public void Execute()
    {
        _optionsNodes[_option()].Execute();
    }
}
