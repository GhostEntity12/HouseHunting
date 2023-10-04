using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampfireButton : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI campfireName;
	public void SetName(string name)
	{
		campfireName.text = name;
	}

	public void AddListener(Map m, CanvasGroup fade, Campfire campfire)
	{
		GetComponent<Button>().onClick.AddListener(() =>
		{
			fade.blocksRaycasts = true;
			LeanTween.alphaCanvas(fade, 1, 0.5f).setOnComplete(() =>
			{
				campfire.SpawnAtCampfire();
				m.CloseMap();
				fade.blocksRaycasts = false;
				LeanTween.alphaCanvas(fade, 0, 0.3f);
			});
		});
	}
}
