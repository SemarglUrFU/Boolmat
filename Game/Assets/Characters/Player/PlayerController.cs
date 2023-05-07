using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputActions inputActions;
 
    private void Awake()
    {
        inputActions = new();
    }

    public PlayerInput GetMovementInput()
    {
        var input = new PlayerInput();
        input.axes = inputActions.Player.Move.ReadValue<Vector2>();
        input.direction = new(Math.Sign(input.axes.x), Math.Sign(input.axes.y));
        input.jump = inputActions.Player.Jump.IsPressed();
        input.dash = inputActions.Player.Dash.IsPressed();
        return input;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}

public class PlayerInput
{
    public Vector2 axes;
    public Vector2Int direction;
    public bool jump;
    public bool dash;
}
