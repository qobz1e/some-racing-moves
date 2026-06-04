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

    void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseUI)
            pauseUI.SetActive(false);
    }

    public void ReturnToMenu()
    {
        if (isExiting) return;

        isExiting = true;
        StartCoroutine(LoadMenu());
    }

    System.Collections.IEnumerator LoadMenu()
    {
        //Time.timeScale = 1f;
        //if (pauseUI) pauseUI.SetActive(false);

        yield return new WaitForSecondsRealtime(returnDelay);

        SceneManager.LoadScene("MainMenu");
    }
}