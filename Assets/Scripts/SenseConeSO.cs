using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Cone_Sense", menuName = "Senses/SenseCone")]
public class SenseConeSO : SenseSO
{
	public float range;
	public float maxAngle;

	public override void DrawGizmo(Transform furniture)
	{
		// Do a sphere check to see if the player is in range of the furniture - don't care about the distance for now
		if (!(Physics.OverlapSphereNonAlloc(furniture.position, range, playerHits, LayerMask.NameToLayer("Player")) == 1)) return;

		Transform player = playerHits[0].transform;


		Vector3 senseDir = Quaternion.Euler(rotOffset) * furniture.forward;
		Vector3 sensePos = furniture.localToWorldMatrix.MultiplyPoint3x4(offset);

		Vector3 toPosition = (player.position - furniture.position).normalized;
		float dist = (player.position - furniture.position).magnitude;
		float angleToPosition = Vector3.Angle(senseDir, toPosition);

		if (angleToPosition <= maxAngle && dist <= range) //&& ((Physics.Raycast((transform.position + new Vector3(0,height,0)), toPosition, out hit, perceptionRadius, finalMask) && hit.collider.CompareTag("Player")) || xray))
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(sensePos, toPosition);
			Gizmos.color = debugDetectedColor;
		}
		else
		{
			Gizmos.color = Color.black;
			Gizmos.DrawRay(sensePos, toPosition);
			Gizmos.color = debugIdleColor;
		}

		Quaternion leftRayRotation = Quaternion.AngleAxis(maxAngle, Vector3.up);
		Quaternion rightRayRotation = Quaternion.AngleAxis(maxAngle, Vector3.up);

		Vector3 leftRayDirection = leftRayRotation * (senseDir) * range;
		Vector3 rightRayDirection = rightRayRotation * (senseDir) * range;
		Vector3 forwardDirection = Vector3.Lerp(leftRayDirection, rightRayDirection, 0.5f);

		float gapLength = (leftRayDirection - rightRayDirection).magnitude;

		Vector3 upRayDirection = forwardDirection + new Vector3(0, gapLength / 2, 0);
		Vector3 downRayDirection = forwardDirection + new Vector3(0, gapLength / -2, 0);

		Gizmos.DrawRay(sensePos, upRayDirection);
		Gizmos.DrawRay(sensePos, downRayDirection);
		Gizmos.DrawRay(sensePos, leftRayDirection);
		Gizmos.DrawRay(sensePos, rightRayDirection);
		Gizmos.DrawLine(sensePos + downRayDirection, sensePos + upRayDirection);
		Gizmos.DrawLine(sensePos + leftRayDirection, sensePos + rightRayDirection);
	}
}
