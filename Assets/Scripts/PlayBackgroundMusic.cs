using UnityEngine;

public class PlayBackgroundMusic : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
