using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class AudioOptionsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private SliderContainer effectsVolumeSlider;
    [SerializeField] private SliderContainer musicVolumeSlider;
    [SerializeField] private SliderContainer masterVolumeSlider;

    public static float MusicVolume { get; private set;}
    public static float SfxVolume { get; private set;}
    
    private void Start() {
        if (PlayerPrefs.HasKey("MasterVolume") || PlayerPrefs.HasKey("MusicVolume") || PlayerPrefs.HasKey("SFXVolume"))
        {
            LoadVolume(masterVolumeSlider);
            LoadVolume(effectsVolumeSlider);
            LoadVolume(musicVolumeSlider);
        }
        else
        {
			SetVolume(masterVolumeSlider);
			SetVolume(effectsVolumeSlider);
			SetVolume(musicVolumeSlider);
		}
    }

    public void SetVolume(SliderContainer sliderContainer) => SetVolume(sliderContainer, sliderContainer.Slider.value);

    public void SetVolume(SliderContainer sliderContainer, float volume)
    {
        sliderContainer.Slider.value = volume;
        float scaledVolume = Mathf.Clamp(volume / 100f, 0.001f, 1);
        myMixer.SetFloat(sliderContainer.Key, Mathf.Log10(scaledVolume) * 20);
        sliderContainer.SliderValueText.text = sliderContainer.Slider.value.ToString();
        PlayerPrefs.SetFloat(sliderContainer.Key, sliderContainer.Slider.value);
    }

    private void LoadVolume(SliderContainer sliderContainer)
    {
        Debug.Log(sliderContainer);
        Debug.Log(sliderContainer.Key);
        Debug.Log(PlayerPrefs.GetFloat(sliderContainer.Key));
        SetVolume(sliderContainer, PlayerPrefs.GetFloat(sliderContainer.Key, 1f));
    }
}
