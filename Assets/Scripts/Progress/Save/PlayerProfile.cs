public static class PlayerProfile
{
    public static int TutorialStep;

    public static bool TutorialCompleted;

    public static int Coins;

    public static int CurrentCarLevel;

    public static CarUpgradeData[] Cars =
    {
        new(),
        new(),
        new(),
        new(),
        new()
    };

    public static bool[] OwnedCars =
    {
        true,
        false,
        false,
        false,
        false
    };

    public static CarColorData[] Colors =
    {
        new(),
        new(),
        new(),
        new(),
        new()
    };

    public static CarUpgradeData Current =>
        Cars[CurrentCarLevel];

    public static CarColorData CurrentColors =>
        Colors[CurrentCarLevel];

    public static int CurrentCarColor =>
        CurrentColors.CurrentColor;

    public static void ApplyDefaultUnlocks()
    {
        Colors[0].UnlockStarterColors();
    }

    public static void ResetProgress()
    {
        Coins = 0;

        CurrentCarLevel = 0;

        for (int i = 0; i < Cars.Length; i++)
            Cars[i] = new CarUpgradeData();

        for (int i = 0; i < OwnedCars.Length; i++)
            OwnedCars[i] = (i == 0);

        for (int i = 0; i < Colors.Length; i++)
            Colors[i] = new CarColorData();

        PlayerUpgrades.LapMoneyLevel = 0;
        PlayerUpgrades.DriftMoneyLevel = 0;
        PlayerUpgrades.PassiveIncomeLevel = 0;

        ApplyDefaultUnlocks();

        SaveSystem.DeleteRaceResults();
        SaveSystem.Save();
    }

    public static void RestartTutorial()
    {
        TutorialCompleted = false;

        SaveSystem.Save();
    }
}