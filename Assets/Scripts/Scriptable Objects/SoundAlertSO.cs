using UnityEngine;

[CreateAssetMenu(fileName = "SoundAlertSO", menuName = "Sound Alert SO")]
public class SoundAlertSO : ScriptableObject
{
	enum SoundType { Impulse, Continuous }

	[SerializeField] private SoundType soundType;

	public bool falloff = true;
	public float range;
	public float volume;

	[HideInInspector] public bool IsContinuous => soundType == SoundType.Continuous;
}
