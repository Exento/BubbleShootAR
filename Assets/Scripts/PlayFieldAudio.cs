using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFieldAudio : MonoBehaviour
{

    public static PlayFieldAudio Instance;  // Singleton instance

    public AudioClip[] hitSounds;
    public AudioClip[] destroySounds;
    public AudioClip[] armorSounds;

    AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void AudioHit()
    {
        audioSource.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)]);
    }

    public void AudioDestroy()
    {
        audioSource.PlayOneShot(destroySounds[Random.Range(0, destroySounds.Length)]);
    }

    public void AudioArmor()
    {
        audioSource.PlayOneShot(destroySounds[Random.Range(0, destroySounds.Length)]);
    }
}
