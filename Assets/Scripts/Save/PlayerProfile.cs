public static class PlayerProfile
{
    public static int Coins;

    public static int SpeedLevel;
    public static int LapMoneyLevel;
    public static int DriftMoneyLevel;
    public static int PassiveIncomeLevel;
    public static int NitroLevel;
    public static int AerodynamicsLevel;

    public static float LapMoneyMultiplier =>
        1f + LapMoneyLevel * 0.5f;

    public static float DriftMoneyMultiplier =>
        1f + DriftMoneyLevel * 0.3f;

    public static int PassiveIncomeAmount =>
        PassiveIncomeLevel <= 0
            ? 0
            : 10 + (PassiveIncomeLevel - 1) * 5;

    public static void ResetProgress()
    {
        Coins = 0;

        SpeedLevel = 0;
        LapMoneyLevel = 0;
        DriftMoneyLevel = 0;
        PassiveIncomeLevel = 0;
        NitroLevel = 0;
        AerodynamicsLevel = 0;

        SaveSystem.Save();
    }
}