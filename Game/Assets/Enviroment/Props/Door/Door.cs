using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : LogicReceiver
{
    [SerializeField] Collider2D doorCollider;
    [SerializeField] Animator animator;
    bool isOpen;

    [SerializeField] UnityEvent OnOpen;
    [SerializeField] UnityEvent OnClose;

    public void ChangeState(bool isOpen)
    {
        if (isOpen == this.isOpen)
            return;
        this.isOpen = isOpen;
        if (isOpen) Open();
        else Close();
    }

    public void Open()
    {
        doorCollider.enabled = false;
        OnOpen.Invoke();
        animator.Play("DoorOpen");
    }

    public void Close()
    {
        doorCollider.enabled = true;
        OnClose.Invoke();
        animator.Play("DoorClose");
    }
}
