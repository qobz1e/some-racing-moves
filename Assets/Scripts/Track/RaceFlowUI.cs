using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceFlowUI : MonoBehaviour
{
    [Header("Leaderboard")]
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private Transform rowsParent;
    [SerializeField] private RaceLeaderboardRow rowPrefab;
    [SerializeField] private Button continueButton;

    [SerializeField] private Sprite goldMedal;
    [SerializeField] private Sprite silverMedal;
    [SerializeField] private Sprite bronzeMedal;

    [Header("Unlock")]
    [SerializeField] private UnlockPanel unlockPanel;

    [Header("Results")]
    [SerializeField] private RaceResultsUI raceResults;

    readonly Queue<System.Action> flowQueue =
        new();

    readonly List<RaceLeaderboardRow> rows =
        new();

    void Awake()
    {
        continueButton.onClick.AddListener(OnLeaderboardContinue);
    }

    public void ShowLeaderboard()
    {
        gameObject.SetActive(true);

        Begin();
    }

    public void Begin()
    {
        BuildLeaderboard();

        leaderboardPanel.SetActive(true);
    }

    void BuildLeaderboard()
    {
        foreach (var r in rows)
            Destroy(r.gameObject);

        rows.Clear();

        foreach (RaceResult result in RaceManager.Instance.Results)
        {
            RaceLeaderboardRow row =
                Instantiate(rowPrefab, rowsParent);

            rows.Add(row);

            Sprite medal = null;

            switch (result.place)
            {
                case 1:
                    medal = goldMedal;
                    break;

                case 2:
                    medal = silverMedal;
                    break;

                case 3:
                    medal = bronzeMedal;
                    break;
            }

            CarVisual visual =
                result.car.GetComponentInChildren<CarVisual>();

            Sprite sprite =
                visual.GetComponent<SpriteRenderer>().sprite;

            row.Setup(
                result,
                medal,
                sprite);
        }
    }

    public void OnLeaderboardContinue()
    {
        leaderboardPanel.SetActive(false);

        BuildFlow();

        ShowNext();
    }

    void BuildFlow()
    {
        flowQueue.Clear();

        if (UnlockSystem.TrackUnlocked)
        {
            flowQueue.Enqueue(() =>
            {
                unlockPanel.ShowTrack(
                    UnlockSystem.TrackIndex,
                    ShowNext);
            });
        }

        if (UnlockSystem.CarUnlocked)
        {
            flowQueue.Enqueue(() =>
            {
                unlockPanel.ShowCar(
                    UnlockSystem.CarIndex,
                    ShowNext);
            });
        }

        flowQueue.Enqueue(() =>
        {
            raceResults.ShowResults();
        });
    }

    void ShowNext()
    {
        if (flowQueue.Count == 0)
            return;

        flowQueue.Dequeue().Invoke();
    }

    public void RefreshLeaderboard()
    {
        BuildLeaderboard();
    }
}