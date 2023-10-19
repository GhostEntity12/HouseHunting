using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private readonly float scaleRange = 1.05f;
    //private bool isHovering = false;
    //private bool isAnimating = false;

    public Image outlineImage; // Drag your outline image here in the Inspector

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(outlineImage.gameObject, Vector3.one * scaleRange, 0.2f)
            .setLoopPingPong()
            .setEase(LeanTweenType.easeInOutSine)
            .setIgnoreTimeScale(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAnimation();
    }

    public void StopAnimation()
    {
        // Reset the outline image scale
        LeanTween.cancel(outlineImage.gameObject);
        outlineImage.rectTransform.localScale = new Vector3(1f, 1.15f, 1f);
    }
}
