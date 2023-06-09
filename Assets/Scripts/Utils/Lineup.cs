using UnityEngine;

public class Lineup : MonoBehaviour
{
    [SerializeField] private float spacing;
    
	private void OnValidate()
	{
		Vector3 pos = Vector3.zero;
		foreach (Transform t in transform)
		{
			t.localPosition = pos;
			pos += Vector3.left * spacing;
		}
	}
}
