using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Map : Singleton<Map>
{
    [Header("UI")]
    [SerializeField] private Canvas mapCanvas;
    [SerializeField] private CampfireButton campfireButtonPrefab;
    [SerializeField] private Image mapBackgroundImageObject;
    [SerializeField] private Button teleportButton;

    [Header("Bounds")]
    [SerializeField] private RectTransform mapBoundCanvas;

    private Campfire[] campfires;
    private Campfire selectedCampfire = null;

    public Campfire SelectedCampfire => selectedCampfire;

    protected override void Awake()
    {
        base.Awake();

        GeneralInputManager.Instance.PlayerInput.Map.CloseMap.performed += ctx => CloseMap();
        campfires = mapBoundCanvas.GetComponentsInChildren<Campfire>();

        foreach (Campfire campfire in campfires)
        {
            CampfireButton campfireButton = Instantiate(campfireButtonPrefab, mapBackgroundImageObject.transform);
            campfireButton.AddListener(campfire);

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

        SelectCampfire(null);
    }

    public void CloseMap()
    {
        GameManager.Instance.HideCursor();
        GeneralInputManager.Instance.PlayerInput.Map.Disable();
        GeneralInputManager.Instance.enabled = true;
        HuntingInputManager.Instance.enabled = true;
        mapCanvas.enabled = false;
    }

    public void SelectCampfire(Campfire campfire)
    {
        selectedCampfire = campfire;
        
        if (selectedCampfire)
        {
            teleportButton.gameObject.SetActive(true);
            teleportButton.GetComponentInChildren<TextMeshProUGUI>().text = "Teleport to " + selectedCampfire.CampfireID;
            teleportButton.onClick.AddListener(() =>
            {
                selectedCampfire.SpawnAtCampfire();
                CloseMap();
            });
        }
        else
        {
            teleportButton.gameObject.SetActive(false);
            teleportButton.onClick.RemoveAllListeners();
        }
    }
}
