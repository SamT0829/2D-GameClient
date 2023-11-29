<<<<<<< HEAD
<<<<<<< HEAD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("環境聲音")]
    public AudioClip ambientClip;
    public AudioClip musicClip;

    [Tooltip("General button click.")]
    [SerializeField] AudioClip buttonClickClip; 
    private Dictionary<string, AudioClip> musicNameMusicClipTable = new Dictionary<string, AudioClip>();

    AudioSource ambientSource;
    AudioSource musicSource;

    public static AudioManager Instance;

    public AudioMixerGroup ambientGroup, musicGroup, FXGroup, playerGroup, voiceGroup;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ambientSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        ambientSource.outputAudioMixerGroup = ambientGroup;
        musicSource.outputAudioMixerGroup = musicGroup;

        // StartLevelAudio();
        AddMusicClip(ambientClip.name, ambientClip);
        PlayMusicClip(ambientClip.name, true);
    }

    public void AddMusicClip(string musicName, AudioClip musicClip)
    {
        musicNameMusicClipTable.Add(musicClip.name, musicClip);
    }

    public void PlayMusicClip(string musicName, bool loop)
    {
        if (musicNameMusicClipTable.TryGetValue(musicName, out AudioClip musicClip))
        {
            musicSource.clip = musicClip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarningFormat("Can't Find music clip with name {0}", musicName);
        }
    }
}
=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("環境聲音")]
    public AudioClip ambientClip;
    public AudioClip musicClip;

    [Tooltip("General button click.")]
    [SerializeField] AudioClip buttonClickClip; 
    private Dictionary<string, AudioClip> musicNameMusicClipTable = new Dictionary<string, AudioClip>();


    AudioSource ambientSource;
    AudioSource musicSource;

    public static AudioManager Instance;

    public AudioMixerGroup ambientGroup, musicGroup, FXGroup, playerGroup, voiceGroup;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ambientSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        ambientSource.outputAudioMixerGroup = ambientGroup;
        musicSource.outputAudioMixerGroup = musicGroup;

        // StartLevelAudio();
        AddMusicClip(ambientClip.name, ambientClip);
        PlayMusicClip(ambientClip.name, true);
    }

    public void AddMusicClip(string musicName, AudioClip musicClip)
    {
        musicNameMusicClipTable.Add(musicClip.name, musicClip);
    }

    public void PlayMusicClip(string musicName, bool loop)
    {
        if (musicNameMusicClipTable.TryGetValue(musicName, out AudioClip musicClip))
        {
            musicSource.clip = musicClip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarningFormat("Can't Find music clip with name {0}", musicName);
        }
    }
}
>>>>>>> b1fb74f0cc798e4b14031ad4091ae67ec4e1be3b
=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("環境聲音")]
    public AudioClip ambientClip;
    public AudioClip musicClip;

    [Tooltip("General button click.")]
    [SerializeField] AudioClip buttonClickClip; 
    private Dictionary<string, AudioClip> musicNameMusicClipTable = new Dictionary<string, AudioClip>();
    private string test;

    AudioSource ambientSource;
    AudioSource musicSource;

    public static AudioManager Instance;

    public AudioMixerGroup ambientGroup, musicGroup, FXGroup, playerGroup, voiceGroup;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ambientSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        ambientSource.outputAudioMixerGroup = ambientGroup;
        musicSource.outputAudioMixerGroup = musicGroup;

        // StartLevelAudio();
        AddMusicClip(ambientClip.name, ambientClip);
        PlayMusicClip(ambientClip.name, true);
    }

    public void AddMusicClip(string musicName, AudioClip musicClip)
    {
        musicNameMusicClipTable.Add(musicClip.name, musicClip);
    }

    public void PlayMusicClip(string musicName, bool loop)
    {
        if (musicNameMusicClipTable.TryGetValue(musicName, out AudioClip musicClip))
        {
            musicSource.clip = musicClip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarningFormat("Can't Find music clip with name {0}", musicName);
        }
    }
}
>>>>>>> parent of f40bf4e (test)
