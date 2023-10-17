using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AssetSeeder : MonoBehaviour
{
	private int prefabCount;
	private GameObject parentObject;
	private Bounds spawnBounds;

	public float yHeight;
	public int maxObjects = 5000;
	public bool instantiateAsPrefab;
	public string parentName;
	public List<GameObject> prefab;

	public LayerMask groundMask;
	public LayerMask layerMask;

	[Min(0)]
	public int maxTries;
	[Min(0)]
	public float spacing;

	[ContextMenu("Seed Object")]
	public void SeedObject()
	{
		if (prefab.Count == 0) { Debug.LogError("Missing a prefab to seed!"); return; }

		spawnBounds = GetComponent<Collider>().bounds;

		if (spacing == 0)
		{
			Debug.LogError("Spacing cannot be zero!");
			return;
		}
		prefabCount = Mathf.Min(Mathf.FloorToInt(spawnBounds.extents.x * spawnBounds.extents.z / (spacing * 3)), maxObjects);
		if (parentObject)
			DestroyImmediate(parentObject);

		parentObject = new GameObject(string.IsNullOrWhiteSpace(parentName) ? $"{prefab[0].name}Parent" : parentName);
		parentObject.transform.position = spawnBounds.center;
		for (int i = 0; i < prefabCount; i++)
		{
			int tries = 0;
			Vector3 randPoint;
			// Gets a random point. Repeats if there is food or an obstacle too close
			do
			{
				randPoint = new Vector3(
					Random.Range(spawnBounds.min.x, spawnBounds.max.x),
					yHeight,
					Random.Range(spawnBounds.min.z, spawnBounds.max.z)
				);

				if (Physics.Raycast(randPoint + Vector3.up * 10, Vector3.down, out RaycastHit hit, 15, groundMask))
				{
					Debug.DrawLine(randPoint + Vector3.up * 10, hit.point, Color.red, 10);
					randPoint = hit.point;
				}

				tries++;
				if (tries > maxTries) break;
			} while (Physics.CheckSphere(randPoint, spacing, layerMask)); // Invalid check
			int randPrefab = Random.Range(0, prefab.Count);
			if (tries <= maxTries)
			{

				#if UNITY_EDITOR
				if (instantiateAsPrefab)
				{
					GameObject obj = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab[randPrefab], parentObject.transform);
					obj.transform.position = randPoint;
					obj.transform.rotation = Quaternion.Euler(0, Random.Range(-180f, 180f), 0);
				}
				else
				{
					Instantiate(prefab[randPrefab], randPoint, Quaternion.Euler(0, Random.Range(-180f, 180f), 0), parentObject.transform);
				}
				#endif
			}
			else
			{
				Debug.LogWarning($"Couldn't find a place to put the {prefab[randPrefab].name}! Seeded {i} {prefab[randPrefab].name} out of a targeted {prefabCount}");
				return;
			}
		}
		Debug.Log($"Sucessfully planted {prefabCount} objects");
	}

	[ContextMenu("Finalise Placement")]
	public void ClearPrev()
	{
		parentObject = null;
	}

}
