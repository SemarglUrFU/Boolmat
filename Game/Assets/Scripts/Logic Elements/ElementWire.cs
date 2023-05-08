using UnityEngine;
using static LogicOperations;

public class ElementWire : LogicElementBase
{
    [SerializeField] LogicInput input;
    [SerializeField] LogicInput output;
    [SerializeField] LineRenderer visual;

    public override void Execute()
    {
        output.Value = Pass(input.Value);

        if (output.Value)
            visual.SetColors(LogicColors.one, LogicColors.one);
        else
            visual.SetColors(LogicColors.zero, LogicColors.zero);
    }
}
