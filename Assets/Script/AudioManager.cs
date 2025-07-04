using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] musicSounds;
    public Sound[] sfxSounds;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public static AudioManager Instance;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
