using UnityEngine;

public enum SenseCategory { Stealth, Normal };

public class SenseSO : ScriptableObject
{
    public Vector3 offset;
    public Vector3 rotOffset;
    public Color debugIdleColor;
    public Color debugDetectedColor;
    public SenseCategory senseCategory;
}
