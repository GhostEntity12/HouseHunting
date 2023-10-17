using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct SenseCone
{
	public Color color;
	public float length;

	[Range(0, 360)]
	public float arc;

	[Range(-180, 180)]
	public float angleOffset;
}

[System.Serializable]
public struct SenseSphere
{
	public Color color;
	public float radius;
}

public class SensesDemo : MonoBehaviour
{
	[SerializeField] private float opacity = 0.5f;
	[SerializeField] private List<SenseSphere> spheres;
	[SerializeField] private List<SenseCone> cones;

	private void OnDrawGizmos()
	{
		Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
		Draw(opacity);
		Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
		Draw(0f);
	}

	private void Draw(float transparency)
	{
		foreach (SenseSphere sphere in spheres)
		{
			Handles.color = new Color(sphere.color.r, sphere.color.g, sphere.color.b, transparency);
			Handles.DrawSolidDisc(transform.position + Vector3.up * 0.01f, Vector3.up, sphere.radius);
		}

		foreach (SenseCone cone in cones)
		{
			Handles.color = new Color(cone.color.r, cone.color.g, cone.color.b, transparency);
			Vector3 baseOffset = Quaternion.Euler(0, cone.angleOffset, 0) * transform.forward;
			Vector3 drawOffset = Quaternion.AngleAxis(-0.5f * cone.arc, Vector3.up) * (baseOffset - Vector3.Dot(baseOffset, Vector3.up) * Vector3.up);
			Handles.DrawSolidArc(transform.position + Vector3.up * 0.01f, Vector3.up, drawOffset, cone.arc, cone.length);
		}
	}
}