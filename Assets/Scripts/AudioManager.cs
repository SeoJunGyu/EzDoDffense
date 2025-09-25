using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource AreaSource;
    [SerializeField] private AudioClip deadSound;
    [SerializeField] private AudioClip sailSound;
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip synthesisSound;

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

    public void PlayDead()
    {
        AreaSource.PlayOneShot(deadSound);
    }

    public void PlaySkill(AudioClip clip)
    {
        if(clip != null)
        {
            AreaSource.PlayOneShot(clip);
        }
    }

    public void PlaySail()
    {
        AreaSource.PlayOneShot(sailSound);
    }

    public void PlaySpawn()
    {
        AreaSource.PlayOneShot(spawnSound);
    }

    public void PlaySynthesis()
    {
        AreaSource.PlayOneShot(synthesisSound);
    }
}
