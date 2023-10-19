using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingText : MonoBehaviour
{
	private TextMeshProUGUI loadingText;
	private void Awake()
	{
		loadingText = GetComponent<TextMeshProUGUI>();
	}

	public void Start()
	{
		StartCoroutine(LoadingTextAnimation());
	}

	public IEnumerator LoadingTextAnimation()
	{
		WaitForSeconds wait = new WaitForSeconds(0.1f);
		int dots = 1;
		while (loadingText)
		{
			loadingText.text = "Loading";
			for (int i = 0; i < dots; i++)
			{
				loadingText.text += ".";
			}
			dots = (dots + 1) % 4;

			yield return wait;
		}
		yield return null;
	}
}
