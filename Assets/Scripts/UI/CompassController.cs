using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassController : MonoBehaviour
{
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private List<CompassMarker> markers = new List<CompassMarker>();

    [SerializeField] private RawImage cardinalImage;

    private float compassUnit;
    private Transform playerTransform;

    public CompassMarker houseMarker;

    private void Start()
    {
        compassUnit = cardinalImage.rectTransform.rect.width / 360f;
        playerTransform = GameManager.Instance.Player.transform;

        AddMarker(houseMarker);
    }

    private void Update()
    {
        if (!playerTransform) return;
        cardinalImage.uvRect = new Rect(playerTransform.localEulerAngles.y / 360f, 0f, 1f, 1f);

        foreach(CompassMarker marker in markers)
        {
            marker.Image.rectTransform.anchoredPosition = GetPosOnCompass(marker);
        }
    }

    public void AddMarker(CompassMarker marker)
    {
        GameObject newMarker = Instantiate(markerPrefab, cardinalImage.transform);
        marker.Image = newMarker.GetComponent<Image>();
        marker.Image.sprite = marker.Icon;

        markers.Add(marker);
    }

    private Vector2 GetPosOnCompass (CompassMarker marker)
    {
        Vector2 playerPos = new Vector2(playerTransform.transform.position.x, playerTransform.transform.position.z);
        Vector2 playerFwd = new Vector2(playerTransform.transform.forward.x, playerTransform.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.Position - playerPos, playerFwd);

        return new Vector2(compassUnit * angle, 0f);
    }
}
