using UnityEngine;

public class PlayerLandingSound : MonoBehaviour
{
    public AudioSource landingSound;
    private bool wasGrounded;

    void Update()
    {
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        if (!wasGrounded && isGrounded)
        {
            landingSound.Play();
        }

        wasGrounded = isGrounded;
    }
}