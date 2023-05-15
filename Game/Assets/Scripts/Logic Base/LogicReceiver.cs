using UnityEngine;
using UnityEngine.Events;

public class LogicReceiver : LogicElementBase
{
    [SerializeField] LogicInput input;
    [SerializeField] UnityEvent<bool> onChangeLogicValue;
    public override void Execute() => onChangeLogicValue.Invoke(input.Value);
}
