using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;
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
        Time.timeScale = 0f;

        if (pauseUI)
            pauseUI.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseUI)
            pauseUI.SetActive(false);
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