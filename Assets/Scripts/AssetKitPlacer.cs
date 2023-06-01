using UnityEditor;
using UnityEngine;

public class AssetKitPlacer : MonoBehaviour
{
	public int m_Count;
	public Vector3 m_Spacing;
# if UNITY_EDITOR
	[ContextMenu("Place")]
	public void PlaceAssets()
	{
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		for (int i = 1; i < m_Count; i++)
		{
			GameObject newObject = PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(gameObject), transform.parent) as GameObject;

			newObject.transform.position = position + m_Spacing;
			newObject.transform.rotation = rotation;

			position = newObject.transform.position;
		}
	}
# endif
}