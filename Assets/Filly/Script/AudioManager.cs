using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioSource uiSource;

    [Header("Sound Database")]
    public SoundDatabase soundDatabase;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mainAudioMixer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayOneShotSFX(SoundType type)
    {
        Sound sound = soundDatabase.GetSound(type);

        if (sound == null || sound.clip == null)
            return;

        sfxSource.PlayOneShot(sound.clip, sound.volume);
    }
}
