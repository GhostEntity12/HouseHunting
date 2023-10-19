using System.Collections;
using UnityEngine;

public class Lure : MonoBehaviour
{
    [SerializeField] private float throwForce;
	[SerializeField] private float throwUpwardForce;

    public static bool lureNotOnCooldown = true;

    private void Awake()
    {
        Rigidbody projectileRb = GetComponent<Rigidbody>();
        Vector3 forceToAdd = Camera.main.transform.forward * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        lureNotOnCooldown = false;
        StartCoroutine(LureTimer());
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(DestroyDelay());
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out WanderAI shootableWanderAI))
            shootableWanderAI.Lure = transform;
    }

    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(8);
        Destroy(gameObject);
    }

	private IEnumerator LureTimer()
    {
        yield return new WaitForSeconds(8);
		lureNotOnCooldown = true;
    }
}
