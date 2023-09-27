using UnityEngine;
using UnityEngine.UI;

public class CampfireButton : MonoBehaviour
{
    public void AddListener(Campfire campfire)
    {
		GetComponent<Button>().onClick.AddListener(() => {
			Map.Instance.SelectCampfire(campfire);
		});
    }    
}
