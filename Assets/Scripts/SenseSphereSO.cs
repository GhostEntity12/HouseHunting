using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Sphere_Sense", menuName = "Senses/SenseSphere")]
public class SenseSphereSO : SenseSO
{
    public float radius;

    public override void DrawGizmo(Transform furniture)
    {
		Gizmos.color = Physics.OverlapSphereNonAlloc(furniture.position, radius, playerHits, LayerMask.NameToLayer("Player")) == 1 ?
			debugDetectedColor :
			debugIdleColor;

		Gizmos.DrawWireSphere(furniture.position, radius);
	}
}