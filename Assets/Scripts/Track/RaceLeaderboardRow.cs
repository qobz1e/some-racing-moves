using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceLeaderboardRow : MonoBehaviour
{
    public Image medalImage;
    public TMP_Text placeText;
    public Image carImage;
    public TMP_Text nameText;
    public TMP_Text timeText;

    public void Setup(
        RaceResult result,
        Sprite medal,
        Sprite carSprite)
    {
        placeText.text = result.place.ToString();

        medalImage.enabled = medal != null;
        medalImage.sprite = medal;

        carImage.sprite = carSprite;

        nameText.text = result.name;

        timeText.text = Format(result.finishTime);
    }

    string Format(float t)
    {
        int m = Mathf.FloorToInt(t / 60);
        int s = Mathf.FloorToInt(t % 60);
        int ms = Mathf.FloorToInt((t * 1000) % 1000);

        return $"{m:00}:{s:00}.{ms:000}";
    }
}