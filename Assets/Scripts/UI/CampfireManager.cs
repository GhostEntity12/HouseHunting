using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireManager : MonoBehaviour
{
	[SerializeField] Transform parent;
	[SerializeField] CampfireButton buttonPrefab;
	[SerializeField] private List<Campfire> campfires;
	// Start is called before the first frame update
	private void Awake()
	{
		campfires.ForEach(c => Instantiate(buttonPrefab, parent).Setup(c, this));
	}

	public void HideCanvas()
	{
		// replace with fade
		gameObject.SetActive(false);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = false;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = true;
		}
	}
}
