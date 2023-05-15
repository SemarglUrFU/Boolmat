using UnityEngine;
using static LogicOperations;

public class ElementAnd : LogicElementBase
{
    [SerializeField] LogicInput input1;
    [SerializeField] LogicInput input2;
    [SerializeField] LogicInput output;
    [SerializeField] SpriteRenderer visual;

    public override void Execute()
    {
        output.Value = And(input1.Value, input2.Value);

        visual.color = (output.Value)
            ? LogicColors.one
            : LogicColors.zero;
    }
}
