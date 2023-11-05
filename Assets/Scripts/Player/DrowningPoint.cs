using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrowningPoint : MonoBehaviour
{
    public bool isDrowning = false;

    // When entering point, have a 3 second delay before losing health
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            StartCoroutine("DrownDelay");     
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            isDrowning = false;
        }
    }

    private IEnumerator DrownDelay()
	{
        Debug.Log("Warning! Get out of the water!");
        isDrowning = true;
        while (GameManager.Instance.Player != null && isDrowning)
        {
            yield return new WaitForSeconds(3);
            HuntingManager.Instance.DealDamageToPlayer(1);
        }
	}
}
