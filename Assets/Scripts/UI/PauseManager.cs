using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private float returnDelay = 0.5f;

    private bool isPaused;
    private bool isExiting;

    void Update()
    {
        if (isExiting) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPaused)
                Pause();
            else
                Resume();
        }
    }

    void Pause()
    {
        isPaused = true;

        Time.timeScale = 0;

        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void Resume()
    {
        isPaused = false;

        Time.timeScale = 1;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackToPause()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void BackToTracksMenu()
    {
        if (isExiting) return;

        isExiting = true;
        StartCoroutine(LoadTracksMenu());
    }

    public void BackToMainMenu()
    {
        if (isExiting) return;

        isExiting = true;
        StartCoroutine(LoadMainMenu());
    }

    System.Collections.IEnumerator LoadMainMenu()
    {
        yield return new WaitForSecondsRealtime(returnDelay);

        SceneManager.LoadScene("MainMenu");

        isPaused = false;
        Time.timeScale = 1f;
    }

    System.Collections.IEnumerator LoadTracksMenu()
    {
        yield return new WaitForSecondsRealtime(returnDelay);

        SceneManager.LoadScene("TracksMenu");

        isPaused = false;
        Time.timeScale = 1f;
    }
}