using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchHeight : MonoBehaviour
{
    [ContextMenu("Match")]
    void Match()
    {
        foreach (Transform t in transform)
        {
            if (Physics.Raycast(new Vector3(t.position.x, 100f, t.position.z), Vector3.down, out RaycastHit hit, 120f, 1<<31))
            {
                t.transform.position = hit.point;
                Debug.Log("hit");
                Debug.DrawRay(hit.point, Vector3.up, Color.red, 10f);
            }
        }
    }
}
