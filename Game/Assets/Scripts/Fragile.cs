using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Fragile : MonoBehaviour
{
    [SerializeField] LayerMask interactMask;
    [SerializeField] float delay = 1;
    [SerializeField] UnityEvent OnTrigger;
    [SerializeField] UnityEvent OnBreak;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if ((1 << collider.gameObject.layer & interactMask.value) == 0) return;
        StartCoroutine(BreakCoroutine());
    }

    private IEnumerator BreakCoroutine() 
    {
        OnTrigger.Invoke();
        yield return new WaitForSeconds(delay);
        OnBreak.Invoke();
        yield return new WaitForSeconds(0.2f);
        foreach (var collider in GetComponents<Collider2D>())
            collider.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
