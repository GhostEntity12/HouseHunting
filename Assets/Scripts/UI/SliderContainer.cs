using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderContainer : MonoBehaviour
{
	[field: SerializeField] public string Key { get; private set; }
	[field: SerializeField] public Slider Slider { get; private set; }
	[field: SerializeField] public TextMeshProUGUI SliderValueText { get; private set; }

    private void Start()
    {
        float sliderValue = PlayerPrefs.GetFloat(Key, 0);
        SliderValueText.text = sliderValue.ToString();
        Slider.value = sliderValue;
        Slider.onValueChanged.AddListener(value =>
        {
            SliderValueText.text = value.ToString();
        });
    }
}
