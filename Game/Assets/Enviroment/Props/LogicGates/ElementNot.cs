using UnityEngine;
using static LogicOperations;

public class ElementNot : LogicElementBase
{
    [SerializeField] LogicInput input;
    [SerializeField] LogicInput output;
    [SerializeField] SpriteRenderer visual;

    public override void Execute()
    {
        output.Value = Not(input.Value);

        visual.color = (output.Value)
            ? LogicColors.one
            : LogicColors.zero;
    }
}
