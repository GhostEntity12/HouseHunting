using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivityManager : MonoBehaviour
{
	private Slider slider;

	private void Awake()
	{
		slider = GetComponent<Slider>();

		slider.onValueChanged.AddListener(value =>
		{
			PlayerPrefs.SetFloat("mouseSensitivity", slider.value);

			if (GameManager.Instance.Player)
				GameManager.Instance.Player.UpdateSensitivity();
		});
	}
}
