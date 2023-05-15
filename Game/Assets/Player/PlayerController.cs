using System;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private PayerAnimation playerAnimation;
    [SerializeField] private LevelController levelController;
    [SerializeField] private LayerMask winLayer;
    [SerializeField] private LayerMask deathLayer;
    private InputActions inputActions;

    public UnityEvent OnDeath;
    public UnityEvent OnWin;

    private void Awake()
    {
        inputActions = new();
    }

    private void Start()
    {
        inputActions.Player.Interact.started += TryInteract;
        inputActions.UI.Close.started += (ctx) => levelController.OpenMenu();
    }

    private void TryInteract(CallbackContext ctx) => playerInteraction.TryInteract();

    public PlayerInput GetMovementInput()
    {
        var input = new PlayerInput();
        input.axes = Filter(inputActions.Player.Move.ReadValue<Vector2>());
        input.direction = new(Math.Sign(input.axes.x), Math.Sign(input.axes.y));
        input.jump = inputActions.Player.Jump.IsPressed();
        input.dash = inputActions.Player.Dash.IsPressed();
        return input;

        static Vector2 Filter(Vector2 position, float epsilon = 0.2f)
        {
            position.x = Math.Abs(position.x) > epsilon ? position.x : 0;
            position.y = Math.Abs(position.y) > epsilon ? position.y : 0;
            return position;
        }
    }

    public void ResetAnimations() => playerAnimation.ResetAnimations();

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleWin(collision);
        HandleDeath(collision);
    }

    private void HandleWin(Collision2D collision)
    {
        if ((1 << collision.gameObject.layer & winLayer.value) == 0)
            return;
        OnWin.Invoke();
        levelController.Win();
    }

    private void HandleDeath(Collision2D collision)
    {
        if ((1 << collision.gameObject.layer & deathLayer.value) == 0)
            return;
        OnDeath.Invoke();
        playerAnimation.Death();
        levelController.RespawnPlayer();
    }
}

public class PlayerInput
{
    public Vector2 axes;
    public Vector2Int direction;
    public bool jump;
    public bool dash;
}
