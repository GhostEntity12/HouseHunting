using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : Singleton<MainMenuManager>
{
    public void NewGame()
    {
        DataPersistenceManager.Instance.NewGame();
        SceneManager.LoadScene("ForestTestingEthan");
    }

    public void Continue()
    {
        DataPersistenceManager.Instance.LoadGame();
        SceneManager.LoadScene("ForestTestingEthan");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
