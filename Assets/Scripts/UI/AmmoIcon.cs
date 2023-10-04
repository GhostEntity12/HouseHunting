using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoIcon : MonoBehaviour
{
	[SerializeField] Image backing;
	[SerializeField] Image icon;
	public float fillAmount => icon.fillAmount;
	public void Initiate(Sprite sprite)
	{
		icon.fillAmount = 1;
		backing.sprite = sprite;
		icon.sprite = sprite;
	}

	public void SetFill(float amount)
	{
		icon.fillAmount = amount;
	}
}
