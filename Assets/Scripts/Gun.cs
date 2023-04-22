using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform gunPoint;
    [SerializeField] private int damage = 10;

    public delegate void OnGunShoot();
    public static event OnGunShoot OnGunShootEvent;

    public void Shoot()
    {
        OnGunShootEvent?.Invoke();
        if (Physics.Raycast(gunPoint.position, gunPoint.forward, out RaycastHit hit, 1000f))
        {
            Shootable shootableTarget = hit.transform.GetComponentInParent<Shootable>();
            if (shootableTarget != null)
                shootableTarget.TakeDamage(damage);
        }
    }
}
