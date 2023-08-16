using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassController : MonoBehaviour
{
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private List<CompassMarker> markers = new List<CompassMarker>();

    [SerializeField] private RawImage cardinalImage;
    [SerializeField] private Transform player;

    float compassUnit;

    public CompassMarker houseMarker;

    private void Start()
    {
        compassUnit = cardinalImage.rectTransform.rect.width / 360f;

        AddMarker(houseMarker);
    }

    private void Update()
    {
        cardinalImage.uvRect = new Rect(player.localEulerAngles.y / 360f, 0f, 1f, 1f);

        foreach(CompassMarker marker in markers)
        {
            marker.image.rectTransform.anchoredPosition = GetPosOnCompass(marker);
        }
    }

    public void AddMarker(CompassMarker marker)
    {
        GameObject newMarker = Instantiate(markerPrefab, cardinalImage.transform);
        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;

        markers.Add(marker);
    }

    Vector2 GetPosOnCompass (CompassMarker marker)
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerFwd = new Vector2(player.transform.forward.x, player.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - playerPos, playerFwd);

        return new Vector2(compassUnit * angle, 0f);
    }
}
