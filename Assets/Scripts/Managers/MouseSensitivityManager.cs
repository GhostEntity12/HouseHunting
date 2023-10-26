using UnityEngine;

public class MouseSensitivityManager : MonoBehaviour
{
	[SerializeField] private SliderContainer slider;

	private void Awake()
	{
		slider.InitAsSensitivity(this);
		PlayerPrefs.Save();
	}

	public void SetSensitivity(SliderContainer sliderContainer, int sensitivity) => sliderContainer.Slider.value = sensitivity;
	public void SetSensitivity(SliderContainer sliderContainer)
	{
		int sensitivity = (int)sliderContainer.Slider.value;
		sliderContainer.SliderValueText.text = sensitivity.ToString();
		PlayerPrefs.SetInt(sliderContainer.Key, sensitivity);

		if (GameManager.Instance.Player)
			GameManager.Instance.Player.UpdateSensitivity();
	}
}
