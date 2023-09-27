using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireManager : MonoBehaviour
{
	[SerializeField] Transform parent;
	[SerializeField] CampfireButton campfireButtonPrefab;
	[SerializeField] private List<Campfire> campfires;

	private void Awake()
	{
	}

    private void Start()
    {
		GameManager.Instance.ShowCursor();
    }

    public void HideCanvas()
	{
		// replace with fade
		gameObject.SetActive(false);
		GameManager.Instance.HideCursor();
		HuntingInputManager.Instance.EnableShooting();
	}
}
