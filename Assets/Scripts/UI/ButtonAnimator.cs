using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image outlineImage; // Drag your outline image here in the Inspector
    //private bool isHovering = false;
    //private bool isAnimating = false;
    private float scaleRange = 1.05f;

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
