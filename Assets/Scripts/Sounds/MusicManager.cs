using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource source;

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip raceMusic;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void PlayMenu()
    {
        Play(menuMusic);
    }

    public void PlayRace()
    {
        Play(raceMusic);
    }

    void Play(AudioClip clip)
    {
        if (source.clip == clip)
            return;

        source.clip = clip;
        source.Play();
    }
}