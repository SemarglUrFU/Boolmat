using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioClip dash;
    [SerializeField] AudioClip jump;
    [SerializeField] AudioClip move;

    bool isGrounded;

    public void Dash() => audioSource.PlayOneShot(dash);
    public void Jump() => audioSource.PlayOneShot(jump);
    public bool SetGrounded(bool isGrounded) => this.isGrounded = isGrounded;
    public void Move(Vector2 speed)
    {
        if (isGrounded && Mathf.Abs(speed.x) > 0.01)
            audioSource.Play();            
    }
}
