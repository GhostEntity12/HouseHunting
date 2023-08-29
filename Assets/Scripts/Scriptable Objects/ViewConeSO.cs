using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ViewCone", menuName = "ViewCone")]
public class ViewConeSO : ScriptableObject
{
	[Tooltip("Positional offset of the cone")]
    public Vector3 posOffset;
	[Tooltip("Rotational offset of the cone")]
    public Vector3 rotOffset;
	[Tooltip("The length of the cone")]
	public float length;
	[Tooltip("The size of the cone"), Range(0, 360)]
	public float angle;
	// The angle actually being used in calculations.
	// The TrueAngle is the permitted angle on either side of the center of the cone
	private float TrueAngle => angle / 2;

#if UNITY_EDITOR
	[Header("Debug")]
    public Color debugIdleColor;
    public Color debugDetectedColor;
#endif

	/// <summary>
	/// Calculate whether a point is in the cone.
	/// </summary>
	/// <param name="cone">The transform the cone is attached to</param>
	/// <param name="point">The point to check</param>
	/// <returns></returns>
	public bool InCone(Transform transform, Vector3 point, bool usesHeight = true)
    {
		// If it shouldn't use height, treat all objects as at y = 0
		Vector3 conePosition = usesHeight ? transform.position : new(transform.position.x, 0, transform.position.z);
		point = usesHeight ? point : new(point.x, 0, point.z);

		// Calculate relative angle and position 
		Vector3 position = conePosition + transform.TransformDirection(posOffset);
		float dist = Vector3.Distance(point, position);
		Vector3 targetDirection = point - position;
		float angle = Mathf.Acos(Vector3.Dot(targetDirection.normalized, Quaternion.Euler(rotOffset) * transform.forward)) * Mathf.Rad2Deg;

		// Return if in range
		return angle < TrueAngle && dist < length;
	}

#if UNITY_EDITOR
	public void DebugDraw(Transform transform, Vector3 point, float transparency, bool usesHeight = true)
	{
		// Change colour if player is in the cone
		Color drawColor = InCone(transform, point, usesHeight) ? debugDetectedColor : debugIdleColor; 
		Handles.color = new Color(drawColor.r, drawColor.g, drawColor.b, transparency);

		// Offset set in view cone
		Vector3 baseOffset = Quaternion.Euler(rotOffset) * transform.forward;
		// Offset to draw it at the correct angle
		Vector3 drawOffset = Quaternion.AngleAxis(-TrueAngle, Vector3.up) * (baseOffset - Vector3.Dot(baseOffset, Vector3.up) * Vector3.up);
		Handles.DrawSolidArc(transform.position + transform.TransformDirection(posOffset) + Vector3.up * 0.01f, Vector3.up, drawOffset, angle, length);
	}
#endif
}
