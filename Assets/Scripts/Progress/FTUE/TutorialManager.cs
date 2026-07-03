using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject welcomePanel;

    [Tooltip("Индекс объекта = TutorialStep")]
    [SerializeField] private GameObject[] stepObjects;

    public TutorialStep Step =>
        (TutorialStep)PlayerProfile.TutorialStep;

    public bool IsTutorialFinished =>
        PlayerProfile.TutorialCompleted;

    void Awake()
    {
        Instance = this;

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (IsTutorialFinished)
        {
            gameObject.SetActive(false);
            return;
        }

        if (welcomePanel != null)
        {
            welcomePanel.SetActive(
                Step == TutorialStep.Welcome);
        }

        ShowCurrentStep();
    }

    public void OnWelcomeAccepted()
    {
        if (welcomePanel != null)
            welcomePanel.SetActive(false);

        NextStep();
    }

    public void NextStep()
    {
        PlayerProfile.TutorialStep++;

        SaveSystem.Save();

        Debug.Log($"FTUE -> {Step}");

        RefreshUI();
    }

    public void FinishTutorial()
    {
        PlayerProfile.TutorialCompleted = true;

        SaveSystem.Save();

        RefreshUI();
    }

    private void ShowCurrentStep()
    {
        if (stepObjects == null)
            return;

        for (int i = 0; i < stepObjects.Length; i++)
        {
            if (stepObjects[i] != null)
            {
                stepObjects[i].SetActive(
                    !IsTutorialFinished &&
                    i == PlayerProfile.TutorialStep);
            }
        }
    }

    //====================================================
    // ACTIONS
    //====================================================

    public void NotifyAction(FtueAction action)
    {
        if (IsTutorialFinished)
            return;

        if (!IsCorrectAction(Step, action))
            return;

        NextStep();
    }

    private bool IsCorrectAction(
        TutorialStep step,
        FtueAction action)
    {
        return step switch
        {
            TutorialStep.OpenGarage1 =>
                action == FtueAction.OpenGarage1,

            TutorialStep.OpenCarsShop =>
                action == FtueAction.OpenCarsShop,

            TutorialStep.ChooseCar =>
                action == FtueAction.ChooseCar,

            TutorialStep.BackToGarage1 =>
                action == FtueAction.BackToGarage1,

            TutorialStep.OpenColorsMenu =>
                action == FtueAction.OpenColorsMenu,

            TutorialStep.ChooseColor =>
                action == FtueAction.ChooseColor,

            TutorialStep.BackToGarage2 =>
                action == FtueAction.BackToGarage2,

            TutorialStep.BackToTracks =>
                action == FtueAction.BackToTracks,

            TutorialStep.StartTrainingRace =>
                action == FtueAction.StartTrainingRace,

            TutorialStep.FinishTrainingRace =>
                action == FtueAction.FinishTrainingRace,

            TutorialStep.OpenGarage2 =>
                action == FtueAction.OpenGarage2,

            TutorialStep.BuyUpgrade =>
                action == FtueAction.BuyUpgrade,

            TutorialStep.FinishTutorial =>
                action == FtueAction.FinishTutorial,

            _ => false
        };
    }

    public void SkipTutorial()
    {
        if (PlayerProfile.TutorialCompleted)
            return;

        PlayerProfile.TutorialStep =
            (int)TutorialStep.FinishTutorial;

        PlayerProfile.TutorialCompleted = true;

        PlayerProfile.Coins += 300;

        SaveSystem.Save();

        RefreshUI();
    }
}