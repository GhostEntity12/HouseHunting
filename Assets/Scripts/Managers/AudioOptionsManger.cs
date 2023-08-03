using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioOptionsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI effectsVolumeText;
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private Slider effectsVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider masterVolumeSlider;

    public static float musicVolume { get; private set;}
    public static float sfxVolume { get; private set;}
    
    private void Start() {
        if (PlayerPrefs.HasKey("MasterVolume") == true || PlayerPrefs.HasKey("MusicVolume") == true || PlayerPrefs.HasKey("SFXVolume") == true )
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
            SetMusicVolume();
            SetEffectVolume();
        }
    }

    public void SetMasterVolume()
    {
        float volume = masterVolumeSlider.value;
        myMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        masterVolumeText.text = ((int)(volume * 100)).ToString();
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    public void SetMusicVolume()
    {
        float volume = musicVolumeSlider.value;
        myMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        musicVolumeText.text = ((int)(volume * 100)).ToString();
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetEffectVolume()
    {
        float volume = effectsVolumeSlider.value;
        myMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        effectsVolumeText.text = ((int)(volume * 100)).ToString();
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadVolume()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        effectsVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetMasterVolume();
        SetMusicVolume();
        SetEffectVolume();
    }
}
