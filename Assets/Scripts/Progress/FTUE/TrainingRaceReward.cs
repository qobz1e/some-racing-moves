using UnityEngine;

public class TrainingRaceReward : MonoBehaviour
{
    [SerializeField] private EconomyManager economy;

    private bool rewardGiven;

    public void GiveReward()
    {
        if (rewardGiven)
            return;

        rewardGiven = true;

        if (PlayerProfile.TutorialCompleted)
            return;

        if (PlayerProfile.TutorialStep !=
            (int)TutorialStep.FinishTrainingRace)
            return;

        economy.AddCoins(300 - PlayerProfile.Coins);
    }
}