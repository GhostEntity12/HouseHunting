using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform gunPoint;
    [SerializeField] private int damage = 10;

    public void Shoot()
    {
        if (Physics.Raycast(gunPoint.position, gunPoint.forward, out RaycastHit hit, 1000f))
        {
            Shootable shootableTarget = hit.transform.GetComponent<Shootable>();
            if (shootableTarget != null)
                shootableTarget.TakeDamage(damage);
        }
    }
}
