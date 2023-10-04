using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using UnityEngine.Rendering;

public class AudioOptionsManager : MonoBehaviour
{
	[SerializeField] private AudioMixer myMixer;
	[SerializeField] private SliderContainer effectsVolumeSlider;
	[SerializeField] private SliderContainer musicVolumeSlider;
	[SerializeField] private SliderContainer masterVolumeSlider;

	public static float MusicVolume { get; private set; }
	public static float SfxVolume { get; private set; }

	private void Start()
	{
		masterVolumeSlider.InitAsAudio(this);
		musicVolumeSlider.InitAsAudio(this);
		effectsVolumeSlider.InitAsAudio(this);
		PlayerPrefs.Save();
	}

	// The sliders have the event which calls the other SetVolume()
	public void SetVolume(SliderContainer sliderContainer, int volume) => sliderContainer.Slider.value = volume;

	public void SetVolume(SliderContainer sliderContainer)
	{
		int volume = (int)sliderContainer.Slider.value;
		float scaledVolume = Mathf.Clamp(volume / 100f, 0.001f, 1);
		myMixer.SetFloat(sliderContainer.Key, Mathf.Log10(scaledVolume) * 20);
		sliderContainer.SliderValueText.text = volume.ToString();
		PlayerPrefs.SetInt(sliderContainer.Key, volume);
	}

	public void SavePrefs() => PlayerPrefs.Save();
}
