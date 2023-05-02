using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject,1.5f);
    }
}
