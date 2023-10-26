using UnityEditor;
using UnityEngine;

public class AssetKitPlacer : MonoBehaviour
{
	public int count;
	public Vector3 spacing;

    #if UNITY_EDITOR
	[ContextMenu("Place")]
	public void PlaceAssets()
	{
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		for (int i = 1; i < count; i++)
		{
			GameObject newObject = PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(gameObject), transform.parent) as GameObject;

			newObject.transform.position = position + spacing;
			newObject.transform.rotation = rotation;

			position = newObject.transform.position;
		}
	}
	#endif
}