using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssetSorter : MonoBehaviour
{
	public enum SortType
	{
		Name,
		xPosPositive,
		xPosNegative,
		yPosPositive,
		yPosNegative,
		zPosPositive,
		zPosNegative,
	}

	public SortType sortType;
	public bool alsoFixNames;
	public string nameCulling;

	[ContextMenu("Sort Children")]
	public void SortChildren()
	{
		if (alsoFixNames) FixNames();

		// Get the index of this object
		int startingIndex = transform.GetSiblingIndex() + 1;

		// Get all the children
		List<Transform> assetsToSort = new List<Transform>();
		foreach (Transform child in transform)
		{
			assetsToSort.Add(child);
		}

		// Remove this object so it's just the children
		assetsToSort.Remove(transform);

		// Sort
		switch (sortType)
		{
			case SortType.Name:
				assetsToSort = assetsToSort.OrderBy(go => go.name).ToList();
				break;
			case SortType.xPosPositive:
				assetsToSort = assetsToSort.OrderBy(go => go.transform.position.x).ToList();
				break;
			case SortType.xPosNegative:
				assetsToSort = assetsToSort.OrderBy(go => go.transform.position.x).Reverse().ToList();
				break;
			case SortType.yPosPositive:
				assetsToSort = assetsToSort.OrderBy(go => go.transform.position.y).ToList();
				break;
			case SortType.yPosNegative:
				assetsToSort = assetsToSort.OrderBy(go => go.transform.position.y).Reverse().ToList();
				break;
			case SortType.zPosPositive:
				assetsToSort = assetsToSort.OrderBy(go => go.transform.position.z).ToList();
				break;
			case SortType.zPosNegative:
				assetsToSort = assetsToSort.OrderBy(go => go.transform.position.z).Reverse().ToList();
				break;
			default:
				break;
		}

		// Reorder
		for (int i = 0; i < assetsToSort.Count; i++)
		{
			assetsToSort[i].SetSiblingIndex(startingIndex + i);
		}

		Debug.Log($"AssetSorter on GameObject <b>{gameObject.name}</b> sorted <b>{assetsToSort.Count}</b> children by sort type <b>{sortType}</b>");
	}

	void FixNames()
	{
		foreach (Transform child in transform)
		{
			if (child.name.Contains("(") && child.name.Contains(")"))
				child.name = child.name.Substring(0, child.name.IndexOf("(") - 1).Trim();
			if (!string.IsNullOrWhiteSpace(nameCulling))
				child.name = child.name.Replace(nameCulling, "");
		}
	}
}