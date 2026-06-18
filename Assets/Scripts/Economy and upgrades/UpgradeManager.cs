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
    public UpgradeData durabilityX;
    public UpgradeData pitstopTime;
    public UpgradeData passiveIncome;
    public UpgradeData nitro;
    public UpgradeData aerodynamics;

    [Header("Cost Scaling")]
    public float costMultiplier = 1.6f;

    [Header("Current Upgrades")]
    public float speedMultiplier = 1f;
    public float lapMoneyMultiplier = 1f;
    public float driftMoneyMultiplier = 1f;

    public float durability = 1000f;
    public float pitstopDuration = 10f;

    public int passiveIncomeAmount;

    public float nitroCapacity;

    public float throttleDecrease;

    private void Awake()
    {
        speed.level = 0;
        lapMoney.level = 0;
        driftMoney.level = 0;
        durabilityX.level = 0;
        pitstopTime.level = 0;
        passiveIncome.level = 0;
        nitro.level = 0;
        aerodynamics.level = 0;

        if (economy == null)
            economy = FindAnyObjectByType<EconomyManager>();

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

    public int GetDurabilityCost() => GetCost(durabilityX.baseCost, durabilityX.level);

    public int GetPitstopTimeCost() => GetCost(pitstopTime.baseCost, pitstopTime.level);

    public int GetPassiveIncomeCost() => GetCost(passiveIncome.baseCost, passiveIncome.level);

    public int GetNitroCost() => GetCost(nitro.baseCost, nitro.level);

    public int GetAerodynamicsCost() => GetCost(aerodynamics.baseCost, aerodynamics.level);

    // =========================
    // BUY METHODS
    // =========================

    private bool TryBuy(ref int level, int maxLevel, int cost)
    {
        if (level >= maxLevel)
            return false;

        if (economy.Coins < cost)
            return false;

        economy.AddCoins(-cost);

        level++;

        ApplyUpgrades();

        return true;
    }

    public void BuySpeedUpgrade()
    {
        TryBuy(
            ref speed.level,
            speed.maxLevel,
            GetSpeedCost()
        );
    }

    public void BuyLapMoneyUpgrade()
    {
        TryBuy(
            ref lapMoney.level,
            lapMoney.maxLevel,
            GetLapMoneyCost()
        );
    }

    public void BuyDriftMoneyUpgrade()
    {
        TryBuy(
            ref driftMoney.level,
            driftMoney.maxLevel,
            GetDriftMoneyCost()
        );
    }

    public void BuyDurabilityUpgrade()
    {
        TryBuy(
            ref durabilityX.level,
            durabilityX.maxLevel,
            GetDurabilityCost()
        );
    }

    public void BuyPitstopTimeUpgrade()
    {
        TryBuy(
            ref pitstopTime.level,
            pitstopTime.maxLevel,
            GetPitstopTimeCost()
        );
    }

    public void BuyPassiveIncomeUpgrade()
    {
        TryBuy(
            ref passiveIncome.level,
            passiveIncome.maxLevel,
            GetPassiveIncomeCost()
        );
    }

    public void BuyNitroUpgrade()
    {
        TryBuy(
            ref nitro.level,
            nitro.maxLevel,
            GetNitroCost()
        );
    }

    public void BuyAerodynamicsUpgrade()
    {
        TryBuy(
            ref aerodynamics.level,
            aerodynamics.maxLevel,
            GetAerodynamicsCost()
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

        // +1 km to base durability
        durability = 
            1000f + durabilityX.level * 500f;

        // -1 s from base duration
        pitstopDuration = 10f - pitstopTime.level;

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
            $"Drift x{driftMoneyMultiplier} | " +
            $"Durability {durability} m | " +
            $"Pitstop {pitstopDuration} s";

        if (passiveIncome.level > 0)
            debugText += $" | Passive {passiveIncomeAmount} coins";

        if (nitro.level > 0)
            debugText += $" | Nitro {nitroCapacity} s";

        if (aerodynamics.level > 0)
            debugText += $" | Drag reduced by {aerodynamics.level * 4}%";

        Debug.Log(debugText);
    }
}