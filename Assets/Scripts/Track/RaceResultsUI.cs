using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceResultsUI : MonoBehaviour
{
    [SerializeField] private RaceManager raceManager;
    [SerializeField] private GameObject panel;

    [SerializeField] private TMP_Text placeText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text bestTimeText;
    [SerializeField] private TMP_Text newBestText;

    private string BestTimeKey =>
        $"BestRaceTime_{SceneManager.GetActiveScene().name}";

    public void ShowResults(int place)
    {
        float finishTime =
            raceManager.CurrentRaceTime;

        panel.SetActive(true);

        placeText.text =
            $"Place: {place}";

        timeText.text =
            $"Time: {FormatTime(finishTime)}";

        bool isNewBest = false;

        float bestTime =
            PlayerPrefs.GetFloat(
                BestTimeKey,
                float.MaxValue
            );

        if (finishTime < bestTime)
        {
            bestTime = finishTime;

            PlayerPrefs.SetFloat(
                BestTimeKey,
                finishTime
            );

            PlayerPrefs.Save();

            isNewBest = true;
        }

        bestTimeText.text =
            $"Best: {FormatTime(bestTime)}";

        newBestText.gameObject.SetActive(isNewBest);
    }

    string FormatTime(float t)
    {
        int minutes =
            Mathf.FloorToInt(t / 60f);

        int seconds =
            Mathf.FloorToInt(t % 60f);

        int milliseconds =
            Mathf.FloorToInt(
                (t * 1000f) % 1000f
            );

        return
            $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }

    public void RestartRace()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("TracksMenu");
    }
}