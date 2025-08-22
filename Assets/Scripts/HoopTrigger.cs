using UnityEngine;

public class HoopTrigger : MonoBehaviour
{
    public AudioClip passSound;
    private AudioSource audioSource;
    private bool triggered = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = passSound;
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            Debug.Log($"Hoop triggered by: {other.name}");

            if (passSound != null)
                audioSource.Play();

            // Hide the parent hoop visuals
            Transform parent = transform.parent;
            if (parent != null)
            {
                Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
                foreach (var r in renderers) r.enabled = false;
            }

            // Disable the trigger
            GetComponent<Collider>().enabled = false;

            // Destroy the whole hoop (parent object) after sound
            Destroy(transform.parent.gameObject, passSound != null ? passSound.length + 0.1f : 0.1f);
        }
    }
}
