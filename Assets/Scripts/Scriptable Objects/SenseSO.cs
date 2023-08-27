using UnityEngine;

[CreateAssetMenu(fileName = "ViewCone", menuName = "ViewCone")]
public class ViewConeSO : ScriptableObject
{
    public Vector3 offset;
    public Vector3 rotOffset;
    public Color debugIdleColor;
    public Color debugDetectedColor;
	public float range;
	public float maxAngle;
}
