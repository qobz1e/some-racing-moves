public static class PlayerProfile
{
    public static int Coins;

    public static int CurrentCarLevel;
    public static int CurrentCarColor;

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

    public static CarUpgradeData Current =>
        Cars[CurrentCarLevel];

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
        CurrentCarColor = 0;

        for (int i = 0; i < Cars.Length; i++)
        {
            Cars[i] = new CarUpgradeData();
        }

        for (int i = 0; i < OwnedCars.Length; i++)
        {
            OwnedCars[i] = (i == 0);
        }

        SaveSystem.DeleteRaceResults();
        SaveSystem.Save();
    }
}