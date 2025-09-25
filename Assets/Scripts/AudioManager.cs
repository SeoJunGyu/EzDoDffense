using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayHit()
    {
        audioSource.PlayOneShot(hitSound);
    }

    public void PlaySkill(AudioClip clip)
    {
        if(clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
