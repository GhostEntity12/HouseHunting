using UnityEngine.Audio;
using UnityEngine;
using System;

[System.Serializable]
public class Sound
{
    public enum AudioTypes { soundEffect, music }
    public AudioTypes audioType;

    [SerializeField] private AudioClip clip;
    [SerializeField] private bool loop;
    [SerializeField] private bool playOnAwake;

    [Range(0f, 1f)] 
    [SerializeField] private float volume = 0.8f;
    [Range(.1f, 3f)] 
    [SerializeField] private float pitch = 1f;

    [SerializeField] private string name;
    private AudioTypes audioTypes;

    [HideInInspector]
    public AudioSource source;

    public string Name => name;
    public AudioTypes Type => audioTypes;

    public void Setup()
    {
        source.clip = clip;
        source.loop = loop;
        source.playOnAwake = playOnAwake;
        source.volume = volume;
        source.pitch = pitch;
        //name = source.name;
    }
}
