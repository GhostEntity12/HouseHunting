using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image outlineImage; // Drag your outline image here in the Inspector
    private bool isHovering = false;

    void Update()
    {
        if (isHovering)
        {
            float jitterAmount = 0.02f; // Change this to adjust the intensity of the jitter
            outlineImage.rectTransform.localScale = new Vector3(1 + Random.Range(-jitterAmount, jitterAmount), 1 + Random.Range(-jitterAmount, jitterAmount), 1f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAnimation();
    }

    public void StopAnimation()
    {
        isHovering = false;
        // Reset the outline image scale
        outlineImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }
}
