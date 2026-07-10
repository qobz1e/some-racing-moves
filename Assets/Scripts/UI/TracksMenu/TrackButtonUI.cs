using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrackButtonUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Image lockIcon;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private Image previewImage;
    [SerializeField] private TMP_Text trackNameText;
    [SerializeField] private TMP_Text bestTimeText;

    [SerializeField] private Image[] stars;
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;

    private TrackDataMenu trackData;

    public void Setup(TrackDataMenu data)
    {
        trackData = data;

        previewImage.sprite = data.previewSprite;
        trackNameText.text = data.trackName;

        float bestTime =
            PlayerPrefs.GetFloat(
                $"BestRaceTime_{data.sceneName}",
                -1f);

        if (bestTime < 0)
        {
            bestTimeText.text = "--:--.---";
            SetStars(0);
            return;
        }

        bestTimeText.text =
            FormatTime(bestTime);

        int starCount = CalculateStars(bestTime);

        SetStars(starCount);
    }

    public void StartRace()
    {
        SceneManager.LoadScene(trackData.sceneName);
    }

    int CalculateStars(float time)
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

    public void SetLocked(bool locked)
    {
        playButton.interactable = !locked;

        if (lockIcon != null)
        {
            Color c = lockIcon.color;
            c.a = locked ? 1f : 0f;
            lockIcon.color = c;
        }

        if (bestTimeText != null)
            bestTimeText.gameObject.SetActive(!locked);

        foreach (Image star in stars)
        {
            if (star != null)
                star.gameObject.SetActive(!locked);
        }

        if (canvasGroup != null)
            canvasGroup.alpha = locked ? 0.45f : 1f;
    }
}