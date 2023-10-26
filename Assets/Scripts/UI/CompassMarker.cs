using UnityEngine;
using UnityEngine.UI;

public class CompassMarker : MonoBehaviour
{
    [SerializeField] private Sprite icon;
    [SerializeField] private Image imageComponent;

    public Sprite Icon => icon;
    public Image Image {  get { return imageComponent; } set { imageComponent = value; } }
    
    public Vector2 Position
    {
        get { return new Vector2(transform.position.x, transform.position.y); }
    }
}
