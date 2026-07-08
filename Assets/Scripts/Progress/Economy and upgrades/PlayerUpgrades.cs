public static class PlayerUpgrades
{
    public static int LapMoneyLevel;
    public static int DriftMoneyLevel;
    public static int PassiveIncomeLevel;

    public static float LapMoneyMultiplier =>
        1f + LapMoneyLevel * 0.5f;

    public static float DriftMoneyMultiplier =>
        1f + DriftMoneyLevel * 0.3f;

    public static int PassiveIncomeAmount =>
        PassiveIncomeLevel <= 0
            ? 0
            : 10 + (PassiveIncomeLevel - 1) * 5;
}