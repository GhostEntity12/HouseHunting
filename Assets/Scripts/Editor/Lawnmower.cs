using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ghost
{
	public class Lawnmower : MonoBehaviour
	{
		[Range(0, 1)] public float mowSensitivity = 0.05f;
		public LayerMask mowArea;

		[ContextMenu("Mow The Lawn")]
		private void Mow()
		{
			List<GameObject> markedForDestroy = new List<GameObject>();
			foreach (Transform item in transform)
			{
				Collider[] overlapColliders = Physics.OverlapSphere(item.position, mowSensitivity, mowArea);
				if (overlapColliders.Count() == 1 && overlapColliders[0].gameObject == item.gameObject) continue; // Don't remove if it's only overlapping itself
				if (overlapColliders.Length > 0)
				{
					markedForDestroy.Add(item.gameObject);
				}
			}

			Debug.Log($"Mowing {markedForDestroy.Count} items");

			foreach (GameObject markedItem in markedForDestroy)
			{
				DestroyImmediate(markedItem);
			}

		}

		[ContextMenu("ChildCount")]
		private void GetChildren()
		{
			print(transform.childCount);
		}
	}
}
