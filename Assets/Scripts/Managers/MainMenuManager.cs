using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuManager : Singleton<MainMenuManager>
{
    [SerializeField] CanvasGroup settingsGroup;
    [SerializeField] VideoPlayer videoPlayer; // Assign this through the inspector
    [SerializeField] RawImage videoOutput; // Assign this through the inspector
    private bool isPlayingCinematic = false; // Flag to check if cinematic is playing

    private void Start()
    {
        AudioManager.Instance.Play("Ambience02");
        if (videoPlayer == null || videoOutput == null)
        {
            Debug.LogError("VideoPlayer or VideoOutput not set.");
            return;
        }
        videoPlayer.loopPointReached += OnCinematicEnded; // Add event for when video ends
        videoOutput.gameObject.SetActive(false); // Initially hide the video output
    }

    public void NewGame()
    {
        DataPersistenceManager.Instance.NewGame();
        PlayCinematic(); // Play the cinematic when New Game is selected
    }

    private void PlayCinematic()
    {
        AudioManager.Instance.Pause("Ambience02");
        videoOutput.gameObject.SetActive(true); // Enable video output to show the video
        videoPlayer.Play(); // Play the cinematic
        isPlayingCinematic = true; // Set flag to true
    }

    private void OnCinematicEnded(VideoPlayer vp)
    {
        if (isPlayingCinematic) // Check if currently playing cinematic
        {
            isPlayingCinematic = false; // Reset flag
            videoOutput.enabled = false; // Disable the video output
            SceneManager.LoadScene(1); // Load the next scene after cinematic ends
        }
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
