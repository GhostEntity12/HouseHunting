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
    [SerializeField] private AudioClip clip;
    [SerializeField] private bool loop;
    [SerializeField] private bool playOnAwake;

    [Range(0f, 1f)] 
    [SerializeField] private float volume = 0.8f;
    [Range(.1f, 3f)] 
    [SerializeField] private float pitch = 1f;

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
