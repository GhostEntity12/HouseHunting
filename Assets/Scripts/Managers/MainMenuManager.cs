using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : Singleton<MainMenuManager>
{
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

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
