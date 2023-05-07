using System;
using UnityEngine;

public class PayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    public void VelocityChanged(Vector2 direction)
    {
        var sign = Math.Sign(direction.x);
        animator.SetBool("IsMovingX", sign != 0);
        animator.SetFloat("VelocityX", sign * Math.Min(Math.Max(Math.Abs(direction.x), 0.5f), 1));
        animator.SetFloat("VelocityY", direction.y);
    }

    public void Grounded(bool isGrounded) => animator.SetBool("IsGrounded", isGrounded);
    public void Dashed(bool isDashing) => animator.SetBool("IsDashing", isDashing);
    public void Death(bool isDead) => animator.SetBool("IsDead", isDead);
    public void WallSliding(bool isWallSliding) => animator.SetBool("IsWallSliding", isWallSliding);
}
