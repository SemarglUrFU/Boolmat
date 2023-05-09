using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private PayerAnimation playerAnimation;
    [SerializeField] private LevelController levelController;
    [SerializeField] private LayerMask deathLayer;
    private InputActions inputActions;

    public UnityEvent OnDeath;
 
    private void Awake()
    {
        inputActions = new();
    }

    private void Start()
    {
        inputActions.Player.Interact.started += TryInteract;
    }

    private void TryInteract(CallbackContext ctx) => playerInteraction.TryInteract();

    public PlayerInput GetMovementInput()
    {
        var input = new PlayerInput();
        input.axes = inputActions.Player.Move.ReadValue<Vector2>();
        input.direction = new(Math.Sign(input.axes.x), Math.Sign(input.axes.y));
        input.jump = inputActions.Player.Jump.IsPressed();
        input.dash = inputActions.Player.Dash.IsPressed();
        return input;
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
        HandleDeath(collision);
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
