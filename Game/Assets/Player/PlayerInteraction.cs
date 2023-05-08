using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private SpriteRenderer inputIcon;
    [SerializeField] private Transform pivot;
    [SerializeField] private float castSize;
    private Interactable selectedInteractable;

    private void Awake()
    {
        inputIcon.enabled = false;
    }

    private void Update()
    {
        var closest = FindClosestInteractable();
        if (selectedInteractable != closest) 
        {
            if (selectedInteractable) selectedInteractable.Deselect();
            if (closest) closest.Select(inputIcon);
            selectedInteractable = closest;
        }
    }

    public void TryInteract()
    {
        if (selectedInteractable)
            selectedInteractable.Interact();
    }

    private Interactable FindClosestInteractable()
    {
        var hits = Physics2D.OverlapCircleAll((Vector2)pivot.position, castSize);
        Interactable closest = null;
        foreach (var hit in hits)
            if (hit.TryGetComponent<Interactable>(out var interactable) 
                && interactable.IsInteractable && (!closest
                || Vector2.Distance(transform.position, closest.transform.position) >
                   Vector2.Distance(transform.position, hit.transform.position)))
                closest = interactable;
        return closest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pivot.position, castSize);
    }
}
