using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UnlockPanel : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject panel;

    [SerializeField] private Image previewImage;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text subtitleText;

    [SerializeField] private Button continueButton;

    [Header("Track Icons")]
    [SerializeField] private TrackDataMenu[] tracks;

    [Header("Cars")]
    [SerializeField] private CarData[] cars;

    Action onContinue;

    void Awake()
    {
        continueButton.onClick.AddListener(Continue);
    }

    public void ShowTrack(
        int trackIndex,
        Action callback)
    {
        panel.SetActive(true);

        onContinue = callback;

        titleText.text = "NEW TRACK";

        subtitleText.text =
            tracks[trackIndex].trackName;

        previewImage.sprite =
            tracks[trackIndex].previewSprite;

        RectTransform rect = previewImage.rectTransform;

        rect.sizeDelta = new Vector2(640, rect.sizeDelta.y);
    }

    public void ShowCar(
        int carIndex,
        Action callback)
    {
        panel.SetActive(true);

        onContinue = callback;

        titleText.text = "NEW CAR";

        subtitleText.text =
            cars[carIndex].carName;

        previewImage.sprite =
            cars[carIndex].icon;

        RectTransform rect = previewImage.rectTransform;

        rect.sizeDelta = new Vector2(240, rect.sizeDelta.y);
    }

    void Continue()
    {
        panel.SetActive(false);

        onContinue?.Invoke();
    }
}