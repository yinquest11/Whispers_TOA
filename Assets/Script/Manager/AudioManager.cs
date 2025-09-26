using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    
    [Header("UI Sliders")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    public bool tempAudioManager;

    [Header("Music Settings")]
    [Tooltip("BGM fade time")]
    public float musicFadeDuration = 2.0f;

    [Header("SFX Settings")]
    
    [SerializeField] float sfxMinPitch = 0.8f;
    
    [SerializeField] float sfxMaxPitch = 1.5f;
    public float sfxVolumeScale = 1f;
    
    [Header("Audio Sources & Clips")]
    public Sound[] musicSounds;
    public Sound[] sfxSounds;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public static AudioManager Instance;
    private Coroutine activeMusicCoroutine; 
    
    private void Awake()
    {
        if (tempAudioManager == true)
            return;
        
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
        
        
        if (tempAudioManager == true)
        {

            // PlayMusic("Menu_Bgm");
            
            return;
        }
           
        
        if (!PlayerPrefs.HasKey("masterVolume")) PlayerPrefs.SetFloat("masterVolume", 1);
        if (!PlayerPrefs.HasKey("musicVolume")) PlayerPrefs.SetFloat("musicVolume", 1);
        if (!PlayerPrefs.HasKey("sfxVolume")) PlayerPrefs.SetFloat("sfxVolume", 1);
        PlayerPrefs.Save();
        Load();
        if (volumeSlider != null) volumeSlider.onValueChanged.AddListener(ChangeMasterVolume);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);
 
        
    }

    public void ChangeMasterVolume(float value)
    {
        PlayerPrefs.SetFloat("masterVolume", value);
        UpdateSfxVolume(); 
        if (activeMusicCoroutine == null) UpdateMusicVolume();
    }

    public void ChangeMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("musicVolume", value);
        if (activeMusicCoroutine == null) UpdateMusicVolume();
    }

    public void ChangeSfxVolume(float value)
    {
        PlayerPrefs.SetFloat("sfxVolume", value);
        UpdateSfxVolume();
    }
    
    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        UpdateMusicVolume();
        UpdateSfxVolume();
    }
    
    private void UpdateMusicVolume()
    {
        float masterVol = volumeSlider != null ? volumeSlider.value : 1f;
        float musicVol = musicSlider != null ? musicSlider.value : 1f;
        musicSource.volume = masterVol * musicVol;
    }
    
    private void UpdateSfxVolume()
    {
        float masterVol = volumeSlider != null ? volumeSlider.value : 1f;
        float sfxVol = sfxSlider != null ? sfxSlider.value : 1f;
        sfxSource.volume = masterVol * sfxVol;
    }


    
    public void PlayMusic(string _name)
    {
        Debug.Log("Changing Bgm to: " + _name);
        
        Sound s = Array.Find(musicSounds, x => x.name == _name);
        if (s == null)
        {
            Debug.LogWarning($"Bgm {_name} not found");
            return;
        }

        if (s.clip == null)
        {
            Debug.LogWarning($"Bgm {_name} no clip");
            return;
        }

        
        if (musicSource.clip == s.clip && musicSource.isPlaying)
        {
            Debug.Log("Return");
            return; 
        }

        
        if (activeMusicCoroutine != null)
        {
            StopCoroutine(activeMusicCoroutine);
        }
        
        activeMusicCoroutine = StartCoroutine(FadeSwitchMusic(s.clip, musicFadeDuration));
    }

  
    private IEnumerator FadeSwitchMusic(AudioClip newClip, float duration)
    {
        
        if (musicSource.isPlaying) 
        {
            float startVolume = musicSource.volume;
            float timer = 0f;

            while (timer < duration)
            {
                
                musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }
        }

        
        musicSource.Stop(); 
        musicSource.volume = 0f;
        musicSource.clip = newClip; 
        musicSource.Play(); 

        
        if (duration > 0)
        {
            float fadeInTimer = 0f;
            while (fadeInTimer < duration)
            {
                
                float masterVol = volumeSlider != null ? volumeSlider.value : 1f;
                float musicVol = musicSlider != null ? musicSlider.value : 1f;
                float targetVolume = masterVol * musicVol;

                
                musicSource.volume = Mathf.Lerp(0f, targetVolume, fadeInTimer / duration);

                fadeInTimer += Time.deltaTime;
                yield return null;
            }
        }
        
        
        UpdateMusicVolume(); 
        activeMusicCoroutine = null; 
    }

    // 
    public void PlaySfx(bool randomPitch,  params string[] _names )
    {
        
        
        if (_names == null || _names.Length == 0)
        {
            Debug.LogWarning("PlaySfx called with empty names");
            return;
        }
        string nameToPlay = _names[UnityEngine.Random.Range(0, _names.Length)];
        Sound s = Array.Find(sfxSounds, x => x.name == nameToPlay);

        if (s == null)
        {
            Debug.LogWarning($"Sfx {nameToPlay} not found");
            return;
        }

        if (s.clip == null)
        {
            Debug.LogWarning($"Sfx {nameToPlay} no clip");
            return;
        }

        
        Debug.Log("Playing Sfx: "+s.clip.name);
        
        if(randomPitch == true)
        {
            
            sfxSource.pitch = UnityEngine.Random.Range(sfxMinPitch, sfxMaxPitch);
            sfxSource.PlayOneShot(s.clip, sfxVolumeScale);
            
        }
        else
        {
            sfxSource.pitch = 1f;
            sfxSource.PlayOneShot(s.clip, sfxVolumeScale);
        }
    }

    
    
    
    public void PlaySfx(params string[] _names) => PlaySfx(true, _names);

    // only one string then is no random pitch
    public void PlaySfx(string _name)
    {
        PlaySfx(false, _name);
    }
    
    

    
}