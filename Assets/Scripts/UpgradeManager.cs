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
    public int maxLevel = 3;

    [Header("Base Costs")]
    public int speedBaseCost = 300;
    public int lapMoneyBaseCost = 500;
    public int driftMoneyBaseCost = 300;

    [Header("Cost Scaling")]
    public float costMultiplier = 1.8f;

    [Header("Current Multipliers")]
    public float speedMultiplier = 1f;
    public float lapMoneyMultiplier = 1f;
    public float driftMoneyMultiplier = 1f;

    private void Awake()
    {
        if (economy == null)
            economy = FindAnyObjectByType<EconomyManager>();

        ApplyUpgrades();
    }

    // =========================
    // COSTS
    // =========================

    public int GetSpeedCost()
    {
        if (speedLevel == 0)
        {
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
        if (lapMoneyLevel == 0)
        {
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
        if (driftMoneyLevel == 0)
        {
            return driftMoneyBaseCost;
        }
        else
        {
            return Mathf.RoundToInt(
                driftMoneyBaseCost * Mathf.Pow(costMultiplier, driftMoneyLevel)
            );
        }
    }

    // =========================
    // BUY METHODS
    // =========================

    public void BuySpeedUpgrade()
    {
        if (speedLevel >= maxLevel)
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
        if (lapMoneyLevel >= maxLevel)
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
        if (driftMoneyLevel >= maxLevel)
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
        // +10% speed per level
        speedMultiplier =
            1f + speedLevel * 0.1f;

        // x1.5 / x2 / x2.5
        lapMoneyMultiplier =
            1f + lapMoneyLevel * 0.5f;

        driftMoneyMultiplier =
            1f + driftMoneyLevel * 0.5f;

        Debug.Log(
            $"Speed x{speedMultiplier} | " +
            $"Lap x{lapMoneyMultiplier} | " +
            $"Drift x{driftMoneyMultiplier}"
        );
    }
}