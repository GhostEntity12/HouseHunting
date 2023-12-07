using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
	[SerializeField] int damage;
	[SerializeField] GameObject bulletHolePrefab;
	[SerializeField] GameObject bloodEffectPrefab;
	[SerializeField] GameObject splinterEffectPrefab;
	[SerializeField] float lifespan;
	[SerializeField] Sprite icon;
	[SerializeField] Sprite magazineIcon;
	[SerializeField] SoundAlertSO collisionSound;

	private TrailRenderer trailRenderer;
	private AmmoPouch ammoPouch;

	private bool hasImpacted = false;

	public Sprite MagazineIcon => magazineIcon;
	public Sprite Icon => icon;
	public Rigidbody Rigidbody { get; private set; }
	public int Damage { get { return damage; } set { damage = value; } }
	public bool CanBounce { get; set; }


	private void Awake()
	{
		Rigidbody = GetComponent<Rigidbody>();
		trailRenderer = GetComponentInChildren<TrailRenderer>();
	}

	private void Update()
	{
		if (gameObject.activeSelf && !hasImpacted && Rigidbody.velocity != Vector3.zero)
		{
			Rigidbody.rotation = Quaternion.LookRotation(Rigidbody.velocity);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		hasImpacted = true;
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

		//instantiate blood and wood chips
        if (collision.transform.tag == "Enemy")
        {
            GameObject bloodEffect = Instantiate(bloodEffectPrefab, contact.point, Quaternion.identity);
			GameObject splinterEffect = Instantiate(splinterEffectPrefab, contact.point, Quaternion.identity);
			bloodEffect.transform.parent = collision.transform;
        }
	}

	public void OnDepool()
	{
		hasImpacted = false;
	}

	private IEnumerator DestroyDelay()
	{
		yield return new WaitForSeconds(CanBounce ? lifespan : 0);
		ammoPouch.PoolBullet(this);
	}

	public void ResetTrail()
	{
		trailRenderer.Clear();
	}

	public void SetPool(AmmoPouch pouch)
	{
		ammoPouch = pouch;
	}
}
