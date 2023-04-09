using UnityEngine;
using UnityEngine.EventSystems;

public class RotationWheel : MonoBehaviour
{

    private float radius = 0f;
    private Transform parent;


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

        parent = transform.GetComponentInParent<Placeable>().transform;
        //get distance from transform to parent to determine the radius of the wheel
        radius = Vector2.Distance(new Vector2(transform.localPosition.x, transform.localPosition.z), new Vector2());
    }

    public void OnDrag(PointerEventData data)
    {
        Ray ray = Camera.main.ScreenPointToRay(data.position);

        Vector3 rayPoint = ray.GetPoint(Vector3.Distance(transform.position, Camera.main.transform.position));

        Vector3 mousePos = new Vector3(rayPoint.x, 0.1f, rayPoint.z);

        float angle = GetAngle(mousePos - parent.position);
        Vector3 normalized = Vector3.Normalize(mousePos - parent.position);
        transform.localPosition = new Vector3(normalized.x * radius, 0.1f, normalized.z * radius);
    }

    public void OnDragEnd()
    {
        transform.localPosition = GetOriginalFixedPoint();
    }

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

    private Vector3 GetOriginalFixedPoint()
    {
        return new Vector3(0f, 0.1f, -radius);
    }
}
