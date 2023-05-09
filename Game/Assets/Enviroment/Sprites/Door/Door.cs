using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : LogicReceiver
{
    [SerializeField] Collider2D doorCollider;
    [SerializeField] Animator animator;

    public void ChangeState(bool isOpen)
    {
        if (isOpen)
            Open();
        else
            Close();
    }

    public void Open()
    {
        doorCollider.enabled = false;
        animator.Play("DoorOpen");
    }

    public void Close()
    {
        doorCollider.enabled = true;
        animator.Play("DoorClose");
    }
}
