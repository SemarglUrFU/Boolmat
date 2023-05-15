using UnityEngine;
using static LogicOperations;

public class ElementNor : LogicElementBase
{
    [SerializeField] LogicInput input1;
    [SerializeField] LogicInput input2;
    [SerializeField] LogicInput output;
    [SerializeField] SpriteRenderer visual;

    public override void Execute()
    {
        output.Value = Nor(input1.Value, input2.Value);

        visual.color = (output.Value)
            ? LogicColors.one
            : LogicColors.zero;
    }
}
