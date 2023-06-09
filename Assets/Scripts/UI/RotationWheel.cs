using UnityEngine;
using UnityEngine.EventSystems;

public class RotationWheel : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private float radius = 0f;
    private Transform parentPlaceableTransform;

    public bool IsRotating { get; private set; }

	private void Awake()
    { 
        parentPlaceableTransform = transform.GetComponentInParent<Placeable>().transform;
	}

	private void OnEnable()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();

        EventTrigger.Entry dragEntry = new();
        EventTrigger.Entry dragEndEntry = new();

        dragEntry.eventID = EventTriggerType.Drag;
        dragEndEntry.eventID = EventTriggerType.EndDrag;

        dragEntry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        dragEndEntry.callback.AddListener((data) => { OnDragEnd(); });

        trigger.triggers.Add(dragEntry);
        trigger.triggers.Add(dragEndEntry);

        //get distance from transform to parent to determine the radius of the wheel
        radius = Vector2.Distance(new Vector2(transform.localPosition.x, transform.localPosition.z), new Vector2());
    }

    private void Update() 
    {
        if (!IsRotating)
            RotateToCamera();
    }

    private void OnDrag(PointerEventData data)
    {
        IsRotating = true;

        Ray ray = Camera.main.ScreenPointToRay(data.position);

        Vector3 rayPoint = ray.GetPoint(Vector3.Distance(transform.position, Camera.main.transform.position));

        Vector3 mousePos = new(rayPoint.x, 0.1f, rayPoint.z);

        //calculate the angle between the fixed point and the mouse position
        float angle = GetAngle(mousePos - parentPlaceableTransform.position, GetOriginalFixedPoint());

        MoveToPositionOnWheel(angle);

        //pass the angle as a negative value to the placeable so that it rotates in the correct direction
        parentPlaceableTransform.GetComponent<Placeable>().RotateToAngle(-angle);
    }

    private void OnDragEnd()
    {
        IsRotating = false;
        transform.localPosition = GetOriginalFixedPoint();
    }

    /// <summary>
    ///  Gets the angle between two points that originate from 0, 0
    /// </summary>
    /// <param name="to">A point on the floor</param>
    /// <returns>A float that represents the angle ranging from 0 - 360</returns>
    private float GetAngle(Vector3 to, Vector3 from)
    {
        float fixedX = from.x;
        float fixedY = from.z;
        float newX = to.x;
        float newY = to.z;

        float angleRadians = Mathf.Atan2(newY, newX) - Mathf.Atan2(fixedY, fixedX);

        if (angleRadians < 0) 
            angleRadians += 2 * Mathf.PI;

        float angleDegrees = angleRadians * 180 / Mathf.PI;

        return angleDegrees;
    }

    private void RotateToCamera()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 placeablePos = parentPlaceableTransform.position;

        float angle = GetAngle(cameraPos - parentPlaceableTransform.position, GetOriginalFixedPoint());

        MoveToPositionOnWheel(angle);
    }

    private void MoveToPositionOnWheel(float angle)
    {
        float x = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        float z = Mathf.Cos(angle * Mathf.Deg2Rad) * -radius;

        transform.localPosition = new Vector3(x, 0.1f, z);
    }

    /// <summary>
    /// Gets the fixed point of the anchor, i.e., the point that is x units in front of the placeable, where x is the radius of the wheel
    /// </summary>
    /// <returns>A Vector3 that contains the point of the anchor</returns>
    private Vector3 GetOriginalFixedPoint()
    {
        return new Vector3(0f, 0.1f, -radius);
    }

    public void SetVisibility(bool visible)
    {
        canvas.enabled = visible;
    }
}
