using UnityEngine;

public class Recoil : MonoBehaviour
{
    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    //Position
    private Vector3 currentPosition;
    private Vector3 targetPosition;
    private Vector3 initialPosition;

    //Recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    //Settings
    [SerializeField] private float kickbackZ;
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    public Vector3 InitialPosition { set { initialPosition = value; } }

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
        if (!WeaponWheel.Instance.GetOpen())
            Camera.main.transform.localRotation = Quaternion.Euler(currentRotation);
        ResetPosition();
    }

    public void RecoilFire()
    {
        targetPosition -= new Vector3(0, 0, kickbackZ);
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    public void ResetPosition()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialPosition, Time.deltaTime * returnSpeed);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snappiness);
        transform.localPosition = currentPosition;
    }
}
