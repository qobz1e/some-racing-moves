using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EconomyManager economy;

    [Header("Upgrade Levels")]
    public int speedLevel;
    public int lapMoneyLevel;
    public int driftMoneyLevel;
    public int durabilityLevel;
    public int pitstopTimeLevel;

    [Header("Max Levels")]
    public int speedMaxLevel = 10;
    public int lapRewardMaxLevel = 3;
    public int driftRewardMaxLevel = 5;
    public int durabilityMaxLevel = 10;
    public int pitstopTimeMaxLevel = 10;

    [Header("Base Costs")]
    public int speedBaseCost = 300;
    public int lapMoneyBaseCost = 300;
    public int driftMoneyBaseCost = 500;
    public int durabilityBaseCost = 300;
    public int pitstopTimeBaseCost = 300;

    [Header("Cost Scaling")]
    public float costMultiplier = 1.8f;

    [Header("Current Upgrades")]
    public float speedMultiplier = 1f;
    public float lapMoneyMultiplier = 1f;
    public float driftMoneyMultiplier = 1f;
    public float durability = 1000f;
    public float pitstopDuration = 10f;

    private void Awake()
    {
        speedLevel = 0;
        lapMoneyLevel = 0;
        driftMoneyLevel = 0;
        durabilityLevel = 0;
        pitstopTimeLevel = 0;

        if (economy == null)
            economy = FindAnyObjectByType<EconomyManager>();

        ApplyUpgrades();
    }

    // =========================
    // COSTS
    // =========================

    public int GetSpeedCost()
    {
        if (speedLevel == 0) {
            return speedBaseCost;
        }
        else
        {
            return Mathf.RoundToInt(
                speedBaseCost * Mathf.Pow(costMultiplier, speedLevel)
            );
        }
    }

    public int GetLapMoneyCost()
    {
        if (lapMoneyLevel == 0) {
            return lapMoneyBaseCost;
        }
        else
        {
            return Mathf.RoundToInt(
                lapMoneyBaseCost * Mathf.Pow(costMultiplier, lapMoneyLevel)
            );
        }
    }

    public int GetDriftMoneyCost()
    {
        if (driftMoneyLevel == 0) {
            return driftMoneyBaseCost;
        }
        else
        {
            return Mathf.RoundToInt(
                driftMoneyBaseCost * Mathf.Pow(costMultiplier, driftMoneyLevel)
            );
        }

    }

    public int GetDurabilityCost()
    {
        if (durabilityLevel == 0) {
            return durabilityBaseCost;
        }
        else
        {
            return Mathf.RoundToInt(
                durabilityBaseCost * Mathf.Pow(costMultiplier, durabilityLevel)
            );
        }

    }

    public int GetPitstopTimeCost()
    {
        if (pitstopTimeLevel == 0)
        {
            return pitstopTimeBaseCost;
        }
        else
        {
            return Mathf.RoundToInt(
                pitstopTimeBaseCost * Mathf.Pow(costMultiplier, pitstopTimeLevel)
            );
        }
    }

    // =========================
    // BUY METHODS
    // =========================

    public void BuySpeedUpgrade()
    {
        if (speedLevel >= speedMaxLevel)
            return;

        int cost = GetSpeedCost();

        if (economy.Coins < cost)
            return;

        economy.AddCoins(-cost);

        speedLevel++;

        ApplyUpgrades();
    }

    public void BuyLapMoneyUpgrade()
    {
        if (lapMoneyLevel >= lapRewardMaxLevel)
            return;

        int cost = GetLapMoneyCost();

        if (economy.Coins < cost)
            return;

        economy.AddCoins(-cost);

        lapMoneyLevel++;

        ApplyUpgrades();
    }

    public void BuyDriftMoneyUpgrade()
    {
        if (driftMoneyLevel >= driftRewardMaxLevel)
            return;

        int cost = GetDriftMoneyCost();

        if (economy.Coins < cost)
            return;

        economy.AddCoins(-cost);

        driftMoneyLevel++;

        ApplyUpgrades();
    }

    public void BuyDurabilityUpgrade()
    {
        if (durabilityLevel >= durabilityMaxLevel)
            return;

        int cost = GetDurabilityCost();

        if (economy.Coins < cost)
            return;

        economy.AddCoins(-cost);

        durabilityLevel++;

        ApplyUpgrades();
    }

    public void BuyPitstopTimeUpgrade()
    {
        if (pitstopTimeLevel >= pitstopTimeMaxLevel)
            return;

        int cost = GetPitstopTimeCost();

        if (economy.Coins < cost)
            return;

        economy.AddCoins(-cost);

        pitstopTimeLevel++;

        ApplyUpgrades();
    }

    // =========================
    // APPLY
    // =========================

    private void ApplyUpgrades()
    {
        // +5% speed per level
        speedMultiplier =
            1f + speedLevel * 0.05f;

        // +50 coins per lap
        lapMoneyMultiplier =
            1f + lapMoneyLevel * 0.5f;

        // +3 coins to base drift reward
        driftMoneyMultiplier =
            1f + driftMoneyLevel * 0.3f;

        // +1 km to base durability
        durability = 
            1000f + durabilityLevel * 500f;

        // -1 s from base duration
        pitstopDuration = 10f - pitstopTimeLevel;

        Debug.Log(
            $"Speed x{speedMultiplier} | " +
            $"Lap x{lapMoneyMultiplier} | " +
            $"Drift x{driftMoneyMultiplier} | " +
            $"Durability {durability} m | " +
            $"Pitstop {pitstopDuration}s"
        );
    }
}