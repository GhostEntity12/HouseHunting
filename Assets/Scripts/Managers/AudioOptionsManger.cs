using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AudioOptionsManager : MonoBehaviour
{
    public static float musicVolume { get; private set;}
    public static float sfxVolume { get; private set;}

    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI effectsVolumeText;

    public void OnMusicVolumeChanged(float value)
    {
        musicVolume = value;
        musicVolumeText.text = Mathf.RoundToInt(value * 100).ToString();
    }
    public void OnSFXVolumeChanged(float value)
    {
        sfxVolume = value;
        effectsVolumeText.text = Mathf.RoundToInt(value * 100).ToString();
    }
}
