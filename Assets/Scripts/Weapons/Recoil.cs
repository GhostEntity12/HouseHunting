using UnityEngine;

public class Recoil : MonoBehaviour
{
    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    //Recoil
    public float recoilX;
    public float recoilY;
    public float recoilZ;

    //Settings
    public float snappiness;
    public float returnSpeed;

    // Start is called before the first frame update
    void Start()
    {
        recoilX = 1;
        recoilY = 1;
        recoilZ = 1;
        snappiness = 3;
        returnSpeed = 2;
    }

    // Update is called once per frame
    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

}
