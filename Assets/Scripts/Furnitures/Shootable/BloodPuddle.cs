using UnityEngine;

public class BloodPuddle : MonoBehaviour
{
    [SerializeField] public int lifeSpan;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeSpan);
    }


}
