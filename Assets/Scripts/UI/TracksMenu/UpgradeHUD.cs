using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeHUD : MonoBehaviour
{
    [SerializeField] private EconomyManager economy;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private GarageCoinsUI coinsUIGarage;
    [SerializeField] private GarageCoinsUI coinsUITracksMenu;
    [SerializeField] private GarageCoinsUI coinsUICarsGarage;
    [SerializeField] private GarageCoinsUI coinsUIColorsMenu;

    [System.Serializable]
    public class UpgradeUI
    {
        public Button button;
        public TMP_Text nameText;
        public TMP_Text priceText;
        public TMP_Text levelText;
        public Image icon;
    }

    [SerializeField] private UpgradeUI engineUI;
    [SerializeField] private UpgradeUI turboUI;
    [SerializeField] private UpgradeUI tiresUI;
    [SerializeField] private UpgradeUI aerodynamicsUI;
    [SerializeField] private UpgradeUI nitroUI;

    [SerializeField] private UpgradeUI lapMoneyUI;
    [SerializeField] private UpgradeUI driftMoneyUI;
    [SerializeField] private UpgradeUI passiveIncomeUI;

    private void Start()
    {
        engineUI.button.onClick.AddListener(BuyEngine);
        turboUI.button.onClick.AddListener(BuyTurbo);
        tiresUI.button.onClick.AddListener(BuyTires);
        aerodynamicsUI.button.onClick.AddListener(BuyAerodynamics);
        nitroUI.button.onClick.AddListener(BuyNitro);

        lapMoneyUI.button.onClick.AddListener(BuyLap);
        driftMoneyUI.button.onClick.AddListener(BuyDrift);
        passiveIncomeUI.button.onClick.AddListener(BuyPassiveIncome);

        economy.OnCoinsChanged += OnCoinsChanged;
        upgradeManager.OnUpgradeChanged += Refresh;

        Refresh();
    }

    private void OnDestroy()
    {
        if (upgradeManager != null)
            upgradeManager.OnUpgradeChanged -= Refresh;

        if (economy != null)
            economy.OnCoinsChanged -= OnCoinsChanged;
    }

    private void OnCoinsChanged(int total, int delta)
    {
        Refresh();
    }

    public void Refresh()
    {
        int engineCost = upgradeManager.GetEngineCost();
        int turboCost = upgradeManager.GetTurboCost();
        int tiresCost = upgradeManager.GetTiresCost();
        int aerodynamicsCost = upgradeManager.GetAerodynamicsCost();
        int nitroCost = upgradeManager.GetNitroCost();

        int lapCost = upgradeManager.GetLapMoneyCost();
        int driftCost = upgradeManager.GetDriftMoneyCost();
        int passiveIncomeCost = upgradeManager.GetPassiveIncomeCost();

        UpdateButton(
            engineUI,
            upgradeManager.engine.level,
            upgradeManager.engine.maxLevel,
            engineCost,
            economy.Coins >= engineCost
        );

        UpdateButton(
            turboUI,
            upgradeManager.turbo.level,
            upgradeManager.turbo.maxLevel,
            turboCost,
            economy.Coins >= turboCost
        );

        UpdateButton(
            tiresUI,
            upgradeManager.tires.level,
            upgradeManager.tires.maxLevel,
            tiresCost,
            economy.Coins >= tiresCost
        );

        UpdateButton(
            aerodynamicsUI,
            upgradeManager.aerodynamics.level,
            upgradeManager.aerodynamics.maxLevel,
            aerodynamicsCost,
            economy.Coins >= aerodynamicsCost
        );

        UpdateButton(
            nitroUI,
            upgradeManager.nitro.level,
            upgradeManager.nitro.maxLevel,
            nitroCost,
            economy.Coins >= nitroCost
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

        coinsUIGarage.Refresh();
        coinsUITracksMenu.Refresh();
        coinsUICarsGarage.Refresh();
        coinsUIColorsMenu.Refresh();
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

    private void BuyEngine()
    {
        upgradeManager.BuyEngineUpgrade();
        Refresh();
    }

    private void BuyTurbo()
    {
        upgradeManager.BuyTurboUpgrade();
        Refresh();
    }

    private void BuyTires()
    {
        upgradeManager.BuyTiresUpgrade();
        Refresh();
    }

    private void BuyAerodynamics()
    {
        upgradeManager.BuyAerodynamicsUpgrade();
        Refresh();
    }

    private void BuyNitro()
    {
        upgradeManager.BuyNitroUpgrade();
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

    private void SetAlpha(Graphic g, float a)
    {
        if (!g) return;

        Color c = g.color;
        c.a = a;
        g.color = c;
    }
}