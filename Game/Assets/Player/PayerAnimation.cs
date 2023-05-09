using System;
using UnityEngine;

public class PayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    public void VelocityChanged(Vector2 direction)
    {
        animator.SetBool("IsMovingX", Math.Abs(direction.x) > 0.0005);
        animator.SetFloat("VelocityX", direction.x);
        animator.SetFloat("VelocityY", direction.y);
    }

    public void Grounded(bool isGrounded) => animator.SetBool("IsGrounded", isGrounded);
    public void Dashed() => animator.SetTrigger("Dash");
    public void Death() => animator.SetTrigger("Dead");
    public void WallSliding(bool isWallSliding) => animator.SetBool("IsWallSliding", isWallSliding);
    public void ResetAnimations()
    {
        Grounded(true);
        WallSliding(false);
        VelocityChanged(Vector2.zero);
        animator.SetTrigger("Reset");
    }
}
