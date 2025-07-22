using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider; 
    [SerializeField] Slider musicSlider;  
    [SerializeField] Slider sfxSlider;    

    public Sound[] musicSounds;
    public Sound[] sfxSounds;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public static AudioManager Instance;

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

    private void Start()
    {
        if (!PlayerPrefs.HasKey("masterVolume"))
        {
            PlayerPrefs.SetFloat("masterVolume", 1);
        }

        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
        }

        if (!PlayerPrefs.HasKey("sfxVolume"))
        {
            PlayerPrefs.SetFloat("sfxVolume", 1);
        }

        PlayerPrefs.Save();

        Load();

        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(ChangeMasterVolume);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(ChangeMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);
    }

    public void ChangeMasterVolume(float value)
    {
        float musicVolume = musicSlider != null ? musicSlider.value : 1f;
        float sfxVolume = sfxSlider != null ? sfxSlider.value : 1f;

        musicSource.volume = value * musicVolume;
        sfxSource.volume = value * sfxVolume;

        PlayerPrefs.SetFloat("masterVolume", value);
        PlayerPrefs.Save();
    }

    public void ChangeMusicVolume(float value)
    {
        float masterVolume = volumeSlider != null ? volumeSlider.value : 1f;
        musicSource.volume = masterVolume * value;
        PlayerPrefs.SetFloat("musicVolume", value);
        PlayerPrefs.Save();
    }

    public void ChangeSfxVolume(float value)
    {
        float masterVolume = volumeSlider != null ? volumeSlider.value : 1f;
        sfxSource.volume = masterVolume * value;
        PlayerPrefs.SetFloat("sfxVolume", value);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        float masterVol = PlayerPrefs.GetFloat("masterVolume");
        float musicVol = PlayerPrefs.GetFloat("musicVolume");
        float sfxVol = PlayerPrefs.GetFloat("sfxVolume");

        if (volumeSlider != null)
            volumeSlider.value = masterVol;
        if (musicSlider != null)
            musicSlider.value = musicVol;
        if (sfxSlider != null)
            sfxSlider.value = sfxVol;

        AudioListener.volume = masterVol;
        musicSource.volume = musicVol;
        sfxSource.volume = sfxVol;
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log($"Music {name} not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySfx(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log($"Sfx {name} not found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }
}
