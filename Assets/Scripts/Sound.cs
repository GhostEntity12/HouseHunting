using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public enum AudioType
    {
        soundEffect,
        music,
    }
    public AudioType audioType;

    public string name;
    public AudioClip clip;
    public bool loop;
    public bool playOnAwake;

    [Range(0f, 1f)] 
    public float volume;
    [Range(.1f, 3f)] 
    public float pitch;

    // public AudioMixerGroup mixerGroup;

    [HideInInspector]
    public AudioSource source;
}
