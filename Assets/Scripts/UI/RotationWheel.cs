using UnityEngine;
using UnityEngine.EventSystems;

public class RotationWheel : MonoBehaviour
{

    private float radius = 0f;
    private Transform parentPlaceableTransform;

    public bool IsRotating { get; private set; }

    private void OnEnable()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();

        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        EventTrigger.Entry dragEndEntry = new EventTrigger.Entry();

        dragEntry.eventID = EventTriggerType.Drag;
        dragEndEntry.eventID = EventTriggerType.EndDrag;

        dragEntry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        dragEndEntry.callback.AddListener((data) => { OnDragEnd(); });

        trigger.triggers.Add(dragEntry);
        trigger.triggers.Add(dragEndEntry);

        parentPlaceableTransform = transform.GetComponentInParent<Placeable>().transform;

        //get distance from transform to parent to determine the radius of the wheel
        radius = Vector2.Distance(new Vector2(transform.localPosition.x, transform.localPosition.z), new Vector2());
    }

    public void OnDrag(PointerEventData data)
    {
        IsRotating = true;

        Ray ray = Camera.main.ScreenPointToRay(data.position);

        Vector3 rayPoint = ray.GetPoint(Vector3.Distance(transform.position, Camera.main.transform.position));

        Vector3 mousePos = new Vector3(rayPoint.x, 0.1f, rayPoint.z);

        //calculate the angle between the fixed point and the mouse position
        float angle = GetAngle(mousePos - parentPlaceableTransform.position);

        //normalize the vector between mouse position and parent placeable to get a fixed radius of one unit for the wheel
        Vector3 normalized = Vector3.Normalize(mousePos - parentPlaceableTransform.position);

        //fix the anchor point to be on the circumference of the wheel with the specified radius
        transform.localPosition = new Vector3(normalized.x * radius, 0.1f, normalized.z * radius);

        //convert the angle to between -180 and 180
        angle = angle > 180 ? angle - 360 : angle;

        //pass the angle as a negative value to the placeable so that it rotates in the correct direction
        parentPlaceableTransform.GetComponent<Placeable>().RotateToAngle(-angle);
    }

    public void OnDragEnd()
    {
        IsRotating = false;
        transform.localPosition = GetOriginalFixedPoint();
    }

    /// <summary>
    ///  Gets the angle between the fixed point and the mouse position
    /// </summary>
    /// <param name="to">A point on the floor</param>
    /// <returns>A float that represents the angle ranging from 0 - 360</returns>
    private float GetAngle(Vector3 to)
    {
        float fixedX = GetOriginalFixedPoint().x;
        float fixedY = GetOriginalFixedPoint().z;
        float newX = to.x;
        float newY = to.z;

        float angleRadians = Mathf.Atan2(newY, newX) - Mathf.Atan2(fixedY, fixedX);

        if (angleRadians < 0) {
            angleRadians += 2 * Mathf.PI;
        }

        float angleDegrees = angleRadians * 180 / Mathf.PI;

        return angleDegrees;
    }

    /// <summary>
    /// Gets the fixed point of the anchor, i.e., the point that is x units in front of the placeable, where x is the radius of the wheel
    /// </summary>
    /// <returns>A Vector3 that contains the point of the anchor</returns>
    private Vector3 GetOriginalFixedPoint()
    {
        return new Vector3(0f, 0.1f, -radius);
    }
}
