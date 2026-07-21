using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RaceResultsUI : MonoBehaviour
{
    [Header("Results UI")]
    [SerializeField] private RaceManager raceManager;
    [SerializeField] private GameObject panel;

    [SerializeField] private TMP_Text placeText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text bestTimeText;
    [SerializeField] private TMP_Text newBestText;

    [Header("Track Data")]
    [SerializeField] private TrackDataMenu trackData;
    [SerializeField] private Image[] stars;
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;

    private string BestTimeKey =>
        $"BestRaceTime_{SceneManager.GetActiveScene().name}";

    public void ShowResults()
    {
        float finishTime =
            RaceManager.Instance.PlayerResult.finishTime;

        int place =
            RaceManager.Instance.PlayerResult.place;

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

        int stars = CalculateStars(finishTime);

        SetStars(stars);

        PlayerProfile.TrackStars[trackData.trackIndex] =
            Mathf.Max(
                PlayerProfile.TrackStars[trackData.trackIndex],
                stars);

        SaveSystem.Save();
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

    public int CalculateStars(float time)
    {
        if (time <= trackData.goldTime)
            return 3;

        if (time <= trackData.silverTime)
            return 2;

        if (time <= trackData.bronzeTime)
            return 1;

        return 0;
    }

    void SetStars(int count)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].sprite =
                i < count
                    ? filledStar
                    : emptyStar;
        }
    }

    void UnlockNext()
    {
        int next = trackData.trackIndex + 1;

        if (!PlayerProfile.UnlockedTracks[next])
        { 
            PlayerProfile.UnlockedTracks[next] = true;

            UnlockSystem.UnlockTrack(next);
        }

        int newCar = next - 1;

        if (!PlayerProfile.UnlockedCars[newCar])
        {
            PlayerProfile.UnlockedCars[newCar] = true;

            UnlockSystem.UnlockCar(newCar);
        }

        SaveSystem.Save();
    }

    public void CheckUnlocks(int stars)
    {
        if (stars == 3)
        {
            UnlockNext();
        }
    }
}