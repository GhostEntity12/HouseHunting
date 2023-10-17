#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Hitbox : MonoBehaviour
{
#if UNITY_EDITOR
	[SerializeField] private bool showHitbox = true;

	private Collider hitbox;

	private void OnValidate()
	{
		hitbox = GetComponent<Collider>();
	}
#endif

	[SerializeField] float damageModifier = 1f;

	private Shootable shootable;
	private WanderAI wanderAI;

	private void Start()
	{
		// Disable the script if there is no collider on the object or if there is no parent shootable
		shootable = transform.GetComponentInParent<Shootable>();
		wanderAI = transform.GetComponentInParent<WanderAI>();
		if (!shootable)
		{
			Debug.LogError("No shootable in parent!", this);
			enabled = false;
			return;
		}

		if (!wanderAI)
		{
			Debug.LogError("No wanderAI in parent!", this);
			enabled = false;
			return;
		}

		if (!TryGetComponent(out Collider _))
		{
			Debug.LogError("Collider missing from hitbox!", this);
			enabled = false;
			return;
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if (showHitbox)
		{   // Get appropriate color
			Color[] colors = { Color.blue, Color.green, Color.red };
			float scaled = Mathf.Clamp(damageModifier, 0, 1.999f);
			Color start = colors[(int)scaled];
			Color end = colors[(int)scaled + 1];
			Color c = Color.Lerp(start, end, scaled - (int)scaled);
			Gizmos.color = new(c.r, c.g, c.b, 0.3f);

			// Render
			switch (hitbox)
			{
				case BoxCollider bc:
					Gizmos.DrawCube(bc.center + GetComponentInParent<Transform>().position, bc.size);
					break;
				case SphereCollider sc:
					Gizmos.DrawSphere(sc.center + GetComponentInParent<Transform>().position, sc.radius);
					break;
				default:
					break;
			}
		}
	}
	[ContextMenu("Normalize")]
	public void NormalizePosition()
	{
		using var editingScope = new PrefabUtility.EditPrefabContentsScope($"Assets/Prefabs/FurnitureShootable/{transform.root.gameObject.name}.prefab");

		GameObject root = editingScope.prefabContentsRoot;

		// Reparent to new position
		Transform newParent = new GameObject("Hitboxes").transform;
		newParent.parent = root.transform;
		Debug.Log(newParent.name);
		Transform oldParent = transform.parent;
		newParent.SetSiblingIndex(oldParent.GetSiblingIndex());
		oldParent = root.transform.GetChild(newParent.GetSiblingIndex() + 1);
		oldParent.name = "OldHitboxes";
		while (oldParent.childCount > 0)
		{
			Transform child = oldParent.GetChild(0);
			// Set position to be at center of collider
			switch (child.GetComponent<Collider>())
			{
				case BoxCollider bc:
					child.localPosition = bc.center + child.localPosition;
					bc.center = Vector3.zero;
					break;
				case SphereCollider sc:
					child.localPosition = sc.center + child.localPosition;
					sc.center = Vector3.zero;
					break;
				case CapsuleCollider cc:
					child.localPosition = cc.center + child.localPosition;
					cc.center = Vector3.zero;
					break;
				default:
					break;
			}
			child.SetParent(newParent);
			child.rotation = Quaternion.identity;
		}
		DestroyImmediate(oldParent.gameObject);
	}
#endif

	public void Damage(int baseDamage)
	{
		// Cast to int, truncates and takes whole number.
		shootable.TakeDamage((int)(baseDamage * damageModifier));
		wanderAI.InducePain();
	}
}