using UnityEngine;
using UnityEngine.UI;

public class AmmoIcon : MonoBehaviour
{
	[SerializeField] Image backing;
	[SerializeField] Image icon;

    public float FillAmount => icon.fillAmount;

	public void Initiate(Sprite sprite)
	{
		icon.fillAmount = 1;
		backing.sprite = sprite;
		icon.sprite = sprite;

		backing.preserveAspect = true;
		icon.preserveAspect = true;
	}

	public void SetFill(float amount)
	{
		icon.fillAmount = amount;
	}
}
