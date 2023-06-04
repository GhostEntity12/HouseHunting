using System.Linq;
using TMPro;
using UnityEngine;

public class FrameCounter : MonoBehaviour
{
	private float[] frameRates;
	private int counter = 0;
	private TextMeshProUGUI text;

	private bool warmingUp = true;
	private Color defaultColor;

	public float smoothing = 1;

	private void Start()
	{
		frameRates = new float[Mathf.FloorToInt(smoothing * 60)];
		text = GetComponent<TextMeshProUGUI>();
		defaultColor = text.color;
		text.color = Color.red;
	}

	private void Update()
	{
		if (warmingUp && counter == frameRates.Length - 1)
		{
			text.color = defaultColor;
			warmingUp = false;
		}
		frameRates[counter] = 1 / Time.deltaTime;
		counter = (counter + 1) % frameRates.Length;
		text.text = Mathf.Floor(frameRates.ToList().Average()).ToString() + " FPS";
	}
}
