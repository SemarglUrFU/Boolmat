using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool IsInteractable { get; set; } = true;

    public virtual void Interact() {}

    [SerializeField] private Transform iconAchor;
    private Transform iconParent;
    private SpriteRenderer icon;

    public virtual void Select(SpriteRenderer icon)
    {
        this.icon = icon;
        iconParent = icon.transform.parent.transform;
        icon.transform.parent = iconAchor.transform;
        icon.transform.position = iconAchor.transform.position;
        icon.enabled = true;
    }

    public virtual void Deselect()
    {
        icon.enabled = false;
        icon.transform.parent = iconParent;
        icon = null;
    }
}
