using UnityEngine;

public enum SenseType { Sight, Sound };
public enum SenseCategory { Quiet, Normal, Loud };

public class SenseSO : ScriptableObject
{
    public Vector3 offset;
    public Vector3 rotOffset;
    public Color debugIdleColor;
    public Color debugDetectedColor;
    public SenseType senseType;
    public SenseCategory senseCategory;
}
