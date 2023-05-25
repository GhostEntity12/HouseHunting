using UnityEngine.Audio;
using UnityEngine;
using System;

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

    public void Setup()
    {
        source.clip = clip;
        source.loop = loop;
        source.playOnAwake = playOnAwake;
        source.volume = volume;
        source.pitch = pitch;
    }

}
