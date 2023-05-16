using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lineup : MonoBehaviour
{
    [SerializeField] float spacing;
    
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
