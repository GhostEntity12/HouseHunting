using UnityEngine.Audio;
using System;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    [SerializeField] public AudioMixerGroup musicMixerGroup;
    [SerializeField] public AudioMixerGroup sfxMixerGroup;
    [SerializeField] public Sound[] sounds;

    // public static AudioManager instance;
    void Awake()
    {
        /* Singleton pattern
        // if there is no instance, set it to this
        if (instance == null) 
            instance = this;
        else 
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        */
        Setup();    
    }

    private void Setup()
    {
        // loop through all sounds and initialize them
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
            
            if (s.playOnAwake)
                s.source.Play();
                
            switch (s.audioType)
            {
                case Sound.AudioType.music:
                    s.source.outputAudioMixerGroup = musicMixerGroup;
                    break;

                case Sound.AudioType.soundEffect:
                    s.source.outputAudioMixerGroup = sfxMixerGroup;
                    break;
            }
        }
    }

    public void Play(string name)
    {
        // find the sound in the array
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null) 
        {
            // if the sound is not found, log a warning and return
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void Pause(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Pause();
    }

    public void UpdateMixerVolume()
    {
        musicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(AudioOptionsManager.musicVolume) * 20);
        sfxMixerGroup.audioMixer.SetFloat("SFXVolume", Mathf.Log10(AudioOptionsManager.sfxVolume) * 20);
    }
}
