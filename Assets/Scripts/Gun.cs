using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform gunPoint;

    public void Shoot()
    {
        if (Physics.Raycast(gunPoint.position, gunPoint.forward, out RaycastHit hit, 1000f))
        {
            Shootable shootableTarget = hit.transform.GetComponent<Shootable>();
            if (shootableTarget != null)
                shootableTarget.TakeDamage(10);
        }
    }
}
