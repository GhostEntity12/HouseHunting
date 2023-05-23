using System.Linq;
using TMPro;
using UnityEngine;

public class FrameCounter : MonoBehaviour
{
	float[] frameRates;
	int counter = 0;
	TextMeshProUGUI m_Text;

	bool m_WarmingUp = true;

	public float m_Smoothing = 1;
	private Color m_DefaultColor;

	// Start is called before the first frame update
	void Start()
	{
		frameRates = new float[Mathf.FloorToInt(m_Smoothing * 60)];
		m_Text = GetComponent<TextMeshProUGUI>();
		m_DefaultColor = m_Text.color;
		m_Text.color = Color.red;
	}

	// Update is called once per frame
	void Update()
	{
		if (m_WarmingUp && counter == frameRates.Length - 1)
		{
			m_Text.color = m_DefaultColor;
			m_WarmingUp = false;
		}
		frameRates[counter] = 1 / Time.deltaTime;
		counter = (counter + 1) % frameRates.Length;
		m_Text.text = Mathf.Floor(frameRates.ToList().Average()).ToString() + " FPS";
	}
}
