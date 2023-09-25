using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
	[SerializeField] int damage;
	[SerializeField] GameObject bulletHolePrefab;
	[SerializeField] float lifespan;
	[SerializeField] SoundAlertSO collisionSound;
	private TrailRenderer trailRenderer;
	private AmmoPouch ammoPouch;

	public Sprite Icon => icon;
	public Rigidbody Rigidbody { get; private set; }
	public int Damage { get { return damage; } set { damage = value; } }
	public bool CanBounce { get; set; }


	private void Awake()
	{
		Rigidbody = GetComponent<Rigidbody>();
		trailRenderer = GetComponentInChildren<TrailRenderer>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.TryGetComponent(out Hitbox hitbox))
		{
			hitbox.Damage(damage);
		}

		StartCoroutine(nameof(DestroyDelay));

		// Instantiate bullet hole
		ContactPoint contact = collision.GetContact(0);
		SoundAlerter.MakeSound(collisionSound, contact.point);
		GameObject bulletHole = Instantiate(bulletHolePrefab, contact.point, Quaternion.LookRotation(contact.normal), collision.transform);
		bulletHole.transform.position += bulletHole.transform.forward / 500;
		Destroy(bulletHole, 1.5f);
	}

	private IEnumerator DestroyDelay()
	{
		yield return new WaitForSeconds(CanBounce ? lifespan : 0);
		ammoPouch.PoolBullet(this);
	}

	public void ResetTrail() => trailRenderer.Clear();
	public void SetPool(AmmoPouch pouch) => ammoPouch = pouch;
}
