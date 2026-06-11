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
    [SerializeField] private UpgradeUI durabilityUI;
    [SerializeField] private UpgradeUI pitstopTimeUI;
    [SerializeField] private UpgradeUI passiveIncomeUI;

    private void Start()
    {
        speedUI.button.onClick.AddListener(BuySpeed);
        lapMoneyUI.button.onClick.AddListener(BuyLap);
        driftMoneyUI.button.onClick.AddListener(BuyDrift);
        durabilityUI.button.onClick.AddListener(BuyDurability);
        pitstopTimeUI.button.onClick.AddListener(BuyPitstopTime);
        passiveIncomeUI.button.onClick.AddListener(BuyPassiveIncome);

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
        int durabilityCost = upgradeManager.GetDurabilityCost();
        int pitstopTimeCost = upgradeManager.GetPitstopTimeCost();
        int passiveIncomeCost = upgradeManager.GetPassiveIncomeCost();

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
            durabilityUI,
            upgradeManager.durabilityX.level,
            upgradeManager.durabilityX.maxLevel,
            durabilityCost,
            economy.Coins >= durabilityCost
        );

        UpdateButton(
            pitstopTimeUI,
            upgradeManager.pitstopTime.level,
            upgradeManager.pitstopTime.maxLevel,
            pitstopTimeCost,
            economy.Coins >= pitstopTimeCost
        );

        UpdateButton(
            passiveIncomeUI,
            upgradeManager.passiveIncome.level,
            upgradeManager.passiveIncome.maxLevel,
            passiveIncomeCost,
            economy.Coins >= passiveIncomeCost
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

    private void BuyDurability()
    {
        upgradeManager.BuyDurabilityUpgrade();
        Refresh();
    }

    private void BuyPitstopTime()
    {
        upgradeManager.BuyPitstopTimeUpgrade();
        Refresh();
    }

    private void BuyPassiveIncome()
    {
        upgradeManager.BuyPassiveIncomeUpgrade();
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