using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private LayerMask playerMask;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if ((1 << collider.gameObject.layer & playerMask.value) == 0) return;
        spawnPoint.position = transform.position;
        Debug.Log($"New spawnpoint is: {spawnPoint.position}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
