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
    public UpgradeData engine;
    public UpgradeData turbo;
    public UpgradeData tires;
    public UpgradeData aerodynamics;
    public UpgradeData nitro;

    public UpgradeData lapMoney;
    public UpgradeData driftMoney;
    public UpgradeData passiveIncome;

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

    public int GetEngineCost() => GetCost(engine.baseCost, engine.level);

    public int GetTurboCost() => GetCost(turbo.baseCost, turbo.level);

    public int GetTiresCost() => GetCost(tires.baseCost, tires.level);

    public int GetAerodynamicsCost() => GetCost(aerodynamics.baseCost, aerodynamics.level);

    public int GetNitroCost() => GetCost(nitro.baseCost, nitro.level);

    public int GetLapMoneyCost() => GetCost(lapMoney.baseCost, lapMoney.level);

    public int GetDriftMoneyCost() => GetCost(driftMoney.baseCost, driftMoney.level);

    public int GetPassiveIncomeCost() => GetCost(passiveIncome.baseCost, passiveIncome.level);

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

    public void BuyEngineUpgrade()
    {
        TryBuy(
            ref engine.level,
            engine.maxLevel,
            GetEngineCost(),
            "Engine"
        );
    }

    public void BuyTurboUpgrade()
    {
        TryBuy(
            ref turbo.level,
            turbo.maxLevel,
            GetTurboCost(),
            "Turbo"
        );
    }

    public void BuyTiresUpgrade()
    {
        TryBuy(
            ref tires.level,
            tires.maxLevel,
            GetTiresCost(),
            "Tires"
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

    public void BuyNitroUpgrade()
    {
        TryBuy(
            ref nitro.level,
            nitro.maxLevel,
            GetNitroCost(),
            "Nitro"
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

    // =========================
    // APPLY
    // =========================

    private void ApplyUpgrades()
    {
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

        string debugText =
            $"Lap x{lapMoneyMultiplier} | " +
            $"Drift x{driftMoneyMultiplier}";

        if (passiveIncome.level > 0)
            debugText += $" | Passive {passiveIncomeAmount} coins";

        Debug.Log(debugText);
    }

    public void Refresh()
    {
        lapMoney.level = PlayerUpgrades.LapMoneyLevel;
        driftMoney.level = PlayerUpgrades.DriftMoneyLevel;
        passiveIncome.level = PlayerUpgrades.PassiveIncomeLevel;

        engine.level = PlayerProfile.Current.EngineLevel;
        turbo.level = PlayerProfile.Current.TurboLevel;
        tires.level = PlayerProfile.Current.TiresLevel;
        aerodynamics.level = PlayerProfile.Current.AerodynamicsLevel;
        nitro.level = PlayerProfile.Current.NitroLevel;

        ApplyUpgrades();
    }

    void SyncProfile()
    {
        PlayerUpgrades.LapMoneyLevel = lapMoney.level;
        PlayerUpgrades.DriftMoneyLevel = driftMoney.level;
        PlayerUpgrades.PassiveIncomeLevel = passiveIncome.level;

        PlayerProfile.Current.EngineLevel = engine.level;
        PlayerProfile.Current.TurboLevel = turbo.level;
        PlayerProfile.Current.TiresLevel = tires.level;
        PlayerProfile.Current.AerodynamicsLevel = aerodynamics.level;
        PlayerProfile.Current.NitroLevel = nitro.level;

        SaveSystem.Save();

        OnUpgradeChanged?.Invoke();
    }
}