using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
#if UNITY_EDITOR
	[SerializeField] bool showHitbox = true;
	Collider hitbox;
	private void OnValidate()
	{
		hitbox = GetComponent<Collider>();
	}
#endif

	[SerializeField] float damageModifier = 1f;

	private Shootable shootable;

	private void Start()
	{
		// Disable the script if there is no collider on the object or if there is no parent shootable
		shootable = transform.GetComponentInParent<Shootable>();
		if (!shootable)
		{
			Debug.LogError("No shootable in parent!", this);
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
			switch (hitbox) {
				case BoxCollider bc:
					Gizmos.DrawCube(bc.center + GetComponentInParent<Transform>().position, bc.size);
					break;
				case SphereCollider sc:
					Gizmos.DrawSphere(sc.center, sc.radius);
					break;
				default:
					break;
			}
		}
	}
#endif

	public void Damage(int baseDamage)
	{
		// Cast to int, truncates and takes whole number.
		shootable.TakeDamage((int)(baseDamage * damageModifier));
	}
}
