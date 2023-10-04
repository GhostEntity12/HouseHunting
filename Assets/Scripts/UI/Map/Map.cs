using UnityEngine;
using UnityEngine.UI;

public class Map : Singleton<Map>
{
    [Header("UI")]
    [SerializeField] private Canvas mapCanvas;
    [SerializeField] private CampfireButton campfireButtonPrefab;
    [SerializeField] private Image mapBackgroundImageObject;
    [SerializeField] private Button teleportButton;
    [SerializeField] private CanvasGroup fade;

    [Header("Bounds")]
    [SerializeField] private RectTransform mapBoundCanvas;

    private Campfire[] campfires;

    protected override void Awake()
    {
        base.Awake();

        GeneralInputManager.Instance.PlayerInput.Map.CloseMap.performed += ctx => CloseMap();
        campfires = mapBoundCanvas.GetComponentsInChildren<Campfire>();

        foreach (Campfire campfire in campfires)
        {
            CampfireButton campfireButton = Instantiate(campfireButtonPrefab, mapBackgroundImageObject.transform);
            campfireButton.AddListener(this, fade, campfire);
            campfireButton.SetName(campfire.CampfireID);

            // Translate the campfire position to the mapBackgroundImage's local space
            Vector3 translatedPosition = TranslateToFixedBound(campfire.transform.localPosition);
            campfireButton.transform.localPosition = translatedPosition;
        }
    }

    private Vector3 TranslateToFixedBound(Vector3 localPos)
    {
        float widthRatio = mapBoundCanvas.rect.width / mapBackgroundImageObject.rectTransform.rect.width;
        float heightRatio = mapBoundCanvas.rect.height / mapBackgroundImageObject.rectTransform.rect.height;

        return new Vector3(
            localPos.x / widthRatio,
            localPos.y / heightRatio,
            localPos.z
        );
    }

    public void OpenMap()
    {
        GameManager.Instance.ShowCursor();
        GeneralInputManager.Instance.PlayerInput.Map.Enable();
        GeneralInputManager.Instance.enabled = false;
        HuntingInputManager.Instance.enabled = false;
        mapCanvas.enabled = true;
    }

    public void CloseMap()
    {
        GameManager.Instance.HideCursor();
        GeneralInputManager.Instance.PlayerInput.Map.Disable();
        GeneralInputManager.Instance.enabled = true;
        HuntingInputManager.Instance.enabled = true;
        mapCanvas.enabled = false;
    }
}
