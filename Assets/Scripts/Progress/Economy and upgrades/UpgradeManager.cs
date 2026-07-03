using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EconomyManager economy;

    [System.Serializable]
    public class UpgradeData
    {
        public int level;
        public int maxLevel;

        public int baseCost;
    }

    [Header("Upgrades")]
    public UpgradeData speed;
    public UpgradeData lapMoney;
    public UpgradeData driftMoney;
    public UpgradeData passiveIncome;
    public UpgradeData nitro;
    public UpgradeData aerodynamics;

    [Header("Cost Scaling")]
    public float costMultiplier = 1.6f;

    [Header("Current Upgrades")]
    public float speedMultiplier = 1f;
    public float lapMoneyMultiplier = 1f;
    public float driftMoneyMultiplier = 1f;

    public int passiveIncomeAmount;

    public float nitroCapacity;

    public float throttleDecrease;

    public System.Action OnUpgradeChanged;

    private void Awake()
    {
        Refresh();

        ApplyUpgrades();
    }

    // =========================
    // COSTS
    // =========================

    private int GetCost(int baseCost, int level)
    {
        return Mathf.RoundToInt(
            baseCost * Mathf.Pow(costMultiplier, level)
        );
    }

    public int GetSpeedCost() => GetCost(speed.baseCost, speed.level);

    public int GetLapMoneyCost() => GetCost(lapMoney.baseCost, lapMoney.level);

    public int GetDriftMoneyCost() => GetCost(driftMoney.baseCost, driftMoney.level);

    public int GetPassiveIncomeCost() => GetCost(passiveIncome.baseCost, passiveIncome.level);

    public int GetNitroCost() => GetCost(nitro.baseCost, nitro.level);

    public int GetAerodynamicsCost() => GetCost(aerodynamics.baseCost, aerodynamics.level);

    // =========================
    // BUY METHODS
    // =========================

    private bool TryBuy(ref int level, int maxLevel, int cost, string saveKey)
    {
        if (level >= maxLevel)
            return false;

        if (economy.Coins < cost)
            return false;

        economy.AddCoins(-cost);

        level++;

        SyncProfile();

        ApplyUpgrades();

        return true;
    }

    public void BuySpeedUpgrade()
    {
        TryBuy(
            ref speed.level,
            speed.maxLevel,
            GetSpeedCost(),
            "Speed"
        );
    }

    public void BuyLapMoneyUpgrade()
    {
        TryBuy(
            ref lapMoney.level,
            lapMoney.maxLevel,
            GetLapMoneyCost(),
            "LapMoney"
        );
    }

    public void BuyDriftMoneyUpgrade()
    {
        TryBuy(
            ref driftMoney.level,
            driftMoney.maxLevel,
            GetDriftMoneyCost(),
            "DriftMoney"
        );
    }

    public void BuyPassiveIncomeUpgrade()
    {
        TryBuy(
            ref passiveIncome.level,
            passiveIncome.maxLevel,
            GetPassiveIncomeCost(),
            "PassiveIncome"
        );
    }

    public void BuyNitroUpgrade()
    {
        TryBuy(
            ref nitro.level,
            nitro.maxLevel,
            GetNitroCost(),
            "Nitro"
        );
    }

    public void BuyAerodynamicsUpgrade()
    {
        TryBuy(
            ref aerodynamics.level,
            aerodynamics.maxLevel,
            GetAerodynamicsCost(),
            "Aerodynamics"
        );
    }

    // =========================
    // APPLY
    // =========================

    private void ApplyUpgrades()
    {
        // +5% speed per level
        speedMultiplier =
            1f + speed.level * 0.05f;

        // +50 coins per lap
        lapMoneyMultiplier =
            1f + lapMoney.level * 0.5f;

        // +3 coins to base drift reward
        driftMoneyMultiplier =
            1f + driftMoney.level * 0.3f;

        // +5 coins per 3s
        passiveIncomeAmount =
            passiveIncome.level <= 0
                ? 0
                : 10 + (passiveIncome.level - 1) * 5;

        // +0.5 s nitro per level
        nitroCapacity = nitro.level > 0 ? 0.5f + nitro.level / 2f : 0;

        // -0.02 throttle drag per level
        throttleDecrease = aerodynamics.level * 0.02f;

        string debugText =
            $"Speed x{speedMultiplier} | " +
            $"Lap x{lapMoneyMultiplier} | " +
            $"Drift x{driftMoneyMultiplier}";

        if (passiveIncome.level > 0)
            debugText += $" | Passive {passiveIncomeAmount} coins";

        if (nitro.level > 0)
            debugText += $" | Nitro {nitroCapacity} s";

        if (aerodynamics.level > 0)
            debugText += $" | Drag reduced by {aerodynamics.level * 4}%";

        Debug.Log(debugText);
    }

    public void Refresh()
    {
        speed.level = PlayerProfile.Current.SpeedLevel;
        lapMoney.level = PlayerProfile.Current.LapMoneyLevel;
        driftMoney.level = PlayerProfile.Current.DriftMoneyLevel;
        passiveIncome.level = PlayerProfile.Current.PassiveIncomeLevel;
        nitro.level = PlayerProfile.Current.NitroLevel;
        aerodynamics.level = PlayerProfile.Current.AerodynamicsLevel;

        ApplyUpgrades();
    }

    void SyncProfile()
    {
        PlayerProfile.Current.SpeedLevel = speed.level;
        PlayerProfile.Current.LapMoneyLevel = lapMoney.level;
        PlayerProfile.Current.DriftMoneyLevel = driftMoney.level;
        PlayerProfile.Current.PassiveIncomeLevel = passiveIncome.level;
        PlayerProfile.Current.NitroLevel = nitro.level;
        PlayerProfile.Current.AerodynamicsLevel = aerodynamics.level;

        SaveSystem.Save();

        OnUpgradeChanged?.Invoke();
    }
}