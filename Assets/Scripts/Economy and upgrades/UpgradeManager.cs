using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EconomyManager economy;

    [Header("Upgrade Levels")]
    public int speedLevel;
    public int lapMoneyLevel;
    public int driftMoneyLevel;

    [Header("Max Levels")]
    public int speedMaxLevel = 5;
    public int lapRewardMaxLevel = 3;
    public int driftRewardMaxLevel = 5;

    [Header("Base Costs")]
    public int speedBaseCost = 300;
    public int lapMoneyBaseCost = 300;
    public int driftMoneyBaseCost = 500;

    [Header("Cost Scaling")]
    public float costMultiplier = 1.8f;

    [Header("Current Multipliers")]
    public float speedMultiplier = 1f;
    public float lapMoneyMultiplier = 1f;
    public float driftMoneyMultiplier = 1f;

    private void Awake()
    {
        speedLevel = 1;
        lapMoneyLevel = 1;
        driftMoneyLevel = 1;

        if (economy == null)
            economy = FindAnyObjectByType<EconomyManager>();

        ApplyUpgrades();
    }

    // =========================
    // COSTS
    // =========================

    public int GetSpeedCost()
    {
        return Mathf.RoundToInt(
            speedBaseCost * Mathf.Pow(costMultiplier, speedLevel - 1)
        );
    }

    public int GetLapMoneyCost()
    {
        return Mathf.RoundToInt(
            lapMoneyBaseCost * Mathf.Pow(costMultiplier, lapMoneyLevel - 1)
        );
    }

    public int GetDriftMoneyCost()
    {
        return Mathf.RoundToInt(
            driftMoneyBaseCost * Mathf.Pow(costMultiplier, driftMoneyLevel - 1)
        );

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

    // =========================
    // APPLY
    // =========================

    private void ApplyUpgrades()
    {
        // +5% speed per level
        speedMultiplier =
            1f + (speedLevel - 1) * 0.05f;

        // +50 coins per lap
        lapMoneyMultiplier =
            1f + (lapMoneyLevel - 1) * 0.5f;

        // +3 coins to base drift reward
        driftMoneyMultiplier =
            1f + (driftMoneyLevel - 1) * 0.3f;

        Debug.Log(
            $"Speed x{speedMultiplier} | " +
            $"Lap x{lapMoneyMultiplier} | " +
            $"Drift x{driftMoneyMultiplier}"
        );
    }
}