using UnityEngine;

public enum SenseType { Sight, Sound };
public enum SenseCategory { Quiet, Normal, Loud };

public class SenseSO : ScriptableObject
{
    public string id;
    public new string name;
    public Vector3 offset;
    public Quaternion rotOffset;
    public Color debugIdleColor;
    public Color debugDetectedColor;
    public SenseType senseType;
    public SenseCategory senseCategory;
}
