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

        var gradient = new Gradient();
        if (output.Value)
            gradient.colorKeys = new GradientColorKey[]{ new (LogicColors.one, 0f), new(LogicColors.one, 1f) };
        else
            gradient.colorKeys = new GradientColorKey[] { new(LogicColors.zero, 0f), new(LogicColors.zero, 1f) };
        visual.colorGradient = gradient;
    }
}
