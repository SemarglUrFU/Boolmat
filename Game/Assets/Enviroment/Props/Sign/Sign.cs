using UnityEngine;

public class Sign : Interactable
{
    [SerializeField] Canvas canvas;
    [SerializeField] PlayerController playerController;
    InputActions input;

    private void Awake()
    {
        input = new();
        input.UI.Close.started += (ctx) => CloseMenu();
    }

    public override void Interact() => OpenMenu();

    void OpenMenu()
    {
        input.Enable();
        playerController.enabled = false;
        canvas.gameObject.SetActive(true);
    }

    void CloseMenu()
    {
        input.Disable();
        canvas.gameObject.SetActive(false);
        playerController.enabled = true;
    }
}
