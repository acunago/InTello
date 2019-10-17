using System.Collections;
using System.Collections.Generic;

public class OptionsDT : IDecision
{
    public delegate int Option();
    Option _option;
    List<IDecision> _optionsNodes;

    public OptionsDT(Option myOption, List<IDecision> myOptionsNodes)
    {
        _option = myOption;
        _optionsNodes = myOptionsNodes;
    }

    public void Execute()
    {
        _optionsNodes[_option()].Execute();
    }
}
