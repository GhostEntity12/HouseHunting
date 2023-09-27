using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lure : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine("DestroyDelay");
    }

    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(8);
        Destroy(gameObject);
    }
}
