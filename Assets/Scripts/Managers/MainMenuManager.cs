using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : Singleton<MainMenuManager>
{
    [SerializeField] CanvasGroup settingsGroup;
	private void Start() 
    {
		AudioManager.Instance.Play("Ambience02");
    }

    public void NewGame()
    {
        DataPersistenceManager.Instance.NewGame();
        SceneManager.LoadScene(1);
    }

    public void Continue()
    {
        DataPersistenceManager.Instance.LoadGame();
        SceneManager.LoadScene(2);
    }


    public void SetSettingsVisible(bool visible)
    {
        settingsGroup.alpha = visible ? 1 : 0;
        settingsGroup.blocksRaycasts = visible;
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
