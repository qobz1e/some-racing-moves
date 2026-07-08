using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeHUDFTUE : MonoBehaviour
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

    private void Start()
    {
        engineUI.button.onClick.AddListener(BuyEngine);

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
        int engineCost = upgradeManager.GetEngineCost();

        UpdateButton(
            engineUI,
            upgradeManager.engine.level,
            upgradeManager.engine.maxLevel,
            engineCost,
            economy.Coins >= engineCost
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

    private void SetAlpha(Graphic g, float a)
    {
        if (!g) return;

        Color c = g.color;
        c.a = a;
        g.color = c;
    }
}