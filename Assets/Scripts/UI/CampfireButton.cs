using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampfireButton : MonoBehaviour
{
	Campfire c;
	[SerializeField] Image icon;
	[SerializeField] TextMeshProUGUI campfireName;

	public void Setup(Campfire c, CampfireManager manager)
	{
		this.c = c;
		icon.sprite = c.CampfireInfo.icon;
		campfireName.SetText(c.CampfireInfo.id);
		Button b = GetComponent<Button>();
		Debug.Log(b, b);
		b.onClick.AddListener(() => { 
			this.c.SpawnAtCampfire();
			manager.HideCanvas();
		});
	}
}
