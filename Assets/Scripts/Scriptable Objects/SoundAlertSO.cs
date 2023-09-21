using UnityEngine;

[CreateAssetMenu(fileName = "SoundAlertSO", menuName = "Sound Alert SO")]
public class SoundAlertSO : ScriptableObject
{
	enum SoundType { Impulse, Continuous }
	public bool falloff = true;
	public float range;
	public float volume;
	[SerializeField] private SoundType soundType;
	[HideInInspector] public bool IsContinuous => soundType == SoundType.Continuous;
}
