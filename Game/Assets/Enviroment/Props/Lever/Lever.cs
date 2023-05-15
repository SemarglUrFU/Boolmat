using UnityEngine;
using UnityEngine.Events;

public class Lever : Interactable
{
    [SerializeField] SpriteRenderer upSprite;
    [SerializeField] SpriteRenderer downSprite;
    [SerializeField] LogicInput logicalInput;
    [SerializeField] bool isActive;

    public UnityEvent OnInteract;

    private void Start()
    {
        UpdateVisuals();
        logicalInput.Value = isActive;
    }

    public override void Interact() 
    {
        isActive = !isActive;
        UpdateVisuals();
        logicalInput.Value = isActive;
        OnInteract.Invoke();
    }

    public void UpdateVisuals()
    {
        if(isActive)
        {
            upSprite.enabled = true;
            downSprite.enabled = false;
        }
        else
        {
            upSprite.enabled = false;
            downSprite.enabled = true;
        }
    }
}
