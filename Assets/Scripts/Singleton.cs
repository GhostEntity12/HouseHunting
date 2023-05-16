using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance { get; private set; }

	// Start is called before the first frame update
	protected virtual void Awake()
	{
		if (Instance != null && Instance != this as T)
		{
			Debug.LogError($"An instance of {typeof(T)} already exists");
			Destroy(this.gameObject);
		}
		else
		{
			// Debug.Log($"Created new instance of singleton {typeof(T)}", this.gameObject);
			Instance = this as T;
		}
	}

	public void Deregister()
	{
		T i = Instance;
		Instance = null;
		Destroy(i.gameObject);
	}
}
