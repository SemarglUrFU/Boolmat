using System;
using UnityEngine;

public class LogicInput : MonoBehaviour
{
    [SerializeField] LogicElementBase element;
    private bool value;
    public bool Value 
    { 
        get => value; 
        set 
        { 
            this.value = value;
            element.Execute();
        }
    }
}
