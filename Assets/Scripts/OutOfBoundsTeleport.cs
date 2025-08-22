using UnityEngine;

public class OutOfBoundsTeleport : MonoBehaviour
{
    public Transform teleportTarget;
    public AudioClip teleportSound;
    public ParticleSystem teleportEffect;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Play teleport effect at current position
            if (teleportEffect != null)
                Instantiate(teleportEffect, other.transform.position, Quaternion.identity);

            // Teleport player
            other.transform.position = teleportTarget.position;
            other.transform.rotation = teleportTarget.rotation;

            // Play sound
            if (teleportSound != null)
                audioSource.PlayOneShot(teleportSound);

            // Play teleport effect at destination
            if (teleportEffect != null)
                Instantiate(teleportEffect, teleportTarget.position, Quaternion.identity);

            Debug.Log("Teleported player to center!");
        }
    }
}
