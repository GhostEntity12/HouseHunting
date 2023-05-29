using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance { get; private set; }

	protected virtual void Awake()
	{
		if (Instance != null && Instance != this as T)
			Destroy(this);
		else
			Instance = this as T;
	}

	public void Deregister()
	{
		T i = Instance;
		Instance = null;
		Destroy(i.gameObject);
	}
}
