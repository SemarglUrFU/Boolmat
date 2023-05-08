using UnityEngine;

public class Lever : Interactable
{
    [SerializeField] SpriteRenderer up;
    [SerializeField] SpriteRenderer down;
    [SerializeField] LogicInput logicalInput;
    bool isActive;

    private void Start()
    {
        isActive = false;
        up.enabled = false;
    }

    public override void Interact() 
    {
        isActive = !isActive;
        if(isActive)
        {
            up.enabled = true;
            down.enabled = false;
        }
        else
        {
            up.enabled = false;
            down.enabled = true;
        }
        logicalInput.Value = isActive;
    }

}
