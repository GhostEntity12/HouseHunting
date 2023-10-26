using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderContainer : MonoBehaviour
{
	[field: SerializeField] public string Key { get; private set; }
	[field: SerializeField] public Slider Slider { get; private set; }
	[field: SerializeField] public TextMeshProUGUI SliderValueText { get; private set; }

	public void InitAsAudio(AudioOptionsManager manager)
	{
		if (PlayerPrefs.HasKey(Key))
		{
			if (PlayerPrefs.GetInt(Key) == Slider.value)
				manager.SetVolume(this);
			else
				manager.SetVolume(this, PlayerPrefs.GetInt(Key));
		}
		else
			manager.SetVolume(this, 80);
	}
	public void InitAsSensitivity(MouseSensitivityManager manager)
	{
		if (PlayerPrefs.HasKey(Key))
		{
			if (PlayerPrefs.GetInt(Key) == Slider.value)
				manager.SetSensitivity(this);
			else
				manager.SetSensitivity(this, PlayerPrefs.GetInt(Key));
		}
		else
			manager.SetSensitivity(this, 10);
	}
}
