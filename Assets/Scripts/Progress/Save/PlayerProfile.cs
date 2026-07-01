public static class PlayerProfile
{
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

    public static float LapMoneyMultiplier =>
        1f + Current.LapMoneyLevel * 0.5f;

    public static float DriftMoneyMultiplier =>
        1f + Current.DriftMoneyLevel * 0.3f;

    public static int PassiveIncomeAmount =>
        Current.PassiveIncomeLevel <= 0
            ? 0
            : 10 + (Current.PassiveIncomeLevel - 1) * 5;

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

        SaveSystem.DeleteRaceResults();
        SaveSystem.Save();
    }
}