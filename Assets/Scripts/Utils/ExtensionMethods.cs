using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
	/// <summary>
	/// Shuffles a IList using the Fisher-Yates method
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = Random.Range(0, n + 1);
			(list[n], list[k]) = (list[k], list[n]);
		}
	}
}