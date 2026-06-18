using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeHUD : MonoBehaviour
{
    [SerializeField] private EconomyManager economy;
    [SerializeField] private UpgradeManager upgradeManager;

    [System.Serializable]
    public class UpgradeUI
    {
        public Button button;
        public TMP_Text nameText;
        public TMP_Text priceText;
        public TMP_Text levelText;
        public Image icon;
    }

    [SerializeField] private UpgradeUI speedUI;
    [SerializeField] private UpgradeUI lapMoneyUI;
    [SerializeField] private UpgradeUI driftMoneyUI;
    [SerializeField] private UpgradeUI passiveIncomeUI;
    [SerializeField] private UpgradeUI nitroUI;
    [SerializeField] private UpgradeUI aerodynamicsUI;

    private void Start()
    {
        speedUI.button.onClick.AddListener(BuySpeed);
        lapMoneyUI.button.onClick.AddListener(BuyLap);
        driftMoneyUI.button.onClick.AddListener(BuyDrift);
        passiveIncomeUI.button.onClick.AddListener(BuyPassiveIncome);
        nitroUI.button.onClick.AddListener(BuyNitro);
        aerodynamicsUI.button.onClick.AddListener(BuyAerodynamics);

        economy.OnCoinsChanged += OnCoinsChanged;

        Refresh();
    }

    private void OnDestroy()
    {
        if (economy != null)
            economy.OnCoinsChanged -= OnCoinsChanged;
    }

    private void OnCoinsChanged(int total, int delta)
    {
        Refresh();
    }

    public void Refresh()
    {
        int speedCost = upgradeManager.GetSpeedCost();
        int lapCost = upgradeManager.GetLapMoneyCost();
        int driftCost = upgradeManager.GetDriftMoneyCost();
        int passiveIncomeCost = upgradeManager.GetPassiveIncomeCost();
        int nitroCost = upgradeManager.GetNitroCost();
        int aerodynamicsCost = upgradeManager.GetAerodynamicsCost();

        UpdateButton(
            speedUI,
            upgradeManager.speed.level,
            upgradeManager.speed.maxLevel,
            speedCost,
            economy.Coins >= speedCost
        );

        UpdateButton(
            lapMoneyUI,
            upgradeManager.lapMoney.level,
            upgradeManager.lapMoney.maxLevel,
            lapCost,
            economy.Coins >= lapCost
        );

        UpdateButton(
            driftMoneyUI,
            upgradeManager.driftMoney.level,
            upgradeManager.driftMoney.maxLevel,
            driftCost,
            economy.Coins >= driftCost
        );

        UpdateButton(
            passiveIncomeUI,
            upgradeManager.passiveIncome.level,
            upgradeManager.passiveIncome.maxLevel,
            passiveIncomeCost,
            economy.Coins >= passiveIncomeCost
        );

        UpdateButton(
            nitroUI,
            upgradeManager.nitro.level,
            upgradeManager.nitro.maxLevel,
            nitroCost,
            economy.Coins >= nitroCost
        );

        UpdateButton(
            aerodynamicsUI,
            upgradeManager.aerodynamics.level,
            upgradeManager.aerodynamics.maxLevel,
            aerodynamicsCost,
            economy.Coins >= aerodynamicsCost
        );
    }

    private void UpdateButton(
        UpgradeUI ui,
        int level,
        int maxLevel,
        int cost,
        bool canBuy)
    {
        if (ui.levelText)
            ui.levelText.text = $"level {level}/{maxLevel}";

        if (level >= maxLevel)
        {
            ui.button.interactable = false;

            SetAlpha(ui.icon, 0.3f);
            SetAlpha(ui.nameText, 0.3f);
            SetAlpha(ui.priceText, 0.3f);
            SetAlpha(ui.levelText, 0.3f);

            if (ui.priceText)
                ui.priceText.text = "MAX";

            return;
        }

        ui.button.interactable = canBuy;

        float a = canBuy ? 1f : 0.5f;

        SetAlpha(ui.icon, a);
        SetAlpha(ui.nameText, a);
        SetAlpha(ui.priceText, a);
        SetAlpha(ui.levelText, a);

        if (ui.priceText)
            ui.priceText.text = cost.ToString();
    }

    private void BuySpeed()
    {
        upgradeManager.BuySpeedUpgrade();
        Refresh();
    }

    private void BuyLap()
    {
        upgradeManager.BuyLapMoneyUpgrade();
        Refresh();
    }

    private void BuyDrift()
    {
        upgradeManager.BuyDriftMoneyUpgrade();
        Refresh();
    }

    private void BuyPassiveIncome()
    {
        upgradeManager.BuyPassiveIncomeUpgrade();
        Refresh();
    }

    private void BuyNitro()
    {
        upgradeManager.BuyNitroUpgrade();
        Refresh();
    }

    private void BuyAerodynamics()
    {
        upgradeManager.BuyAerodynamicsUpgrade();
        Refresh();
    }

    private void SetAlpha(Graphic g, float a)
    {
        if (!g) return;

        Color c = g.color;
        c.a = a;
        g.color = c;
    }
}