using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeHUD : MonoBehaviour
{
    [SerializeField] private EconomyManager economy;
    [SerializeField] private UpgradeManager upgradeManager;

    [SerializeField] private Button speedBtn;
    [SerializeField] private Button lapBtn;
    [SerializeField] private Button driftBtn;

    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text lapText;
    [SerializeField] private TMP_Text driftText;

    [SerializeField] private TMP_Text speedPrice;
    [SerializeField] private TMP_Text lapPrice;
    [SerializeField] private TMP_Text driftPrice;

    [SerializeField] private TMP_Text speedLevel;
    [SerializeField] private TMP_Text lapLevel;
    [SerializeField] private TMP_Text driftLevel;

    [SerializeField] private Image speedImg;
    [SerializeField] private Image lapImg;
    [SerializeField] private Image driftImg;

    private void Start()
    {
        speedBtn.onClick.AddListener(BuySpeed);
        lapBtn.onClick.AddListener(BuyLap);
        driftBtn.onClick.AddListener(BuyDrift);

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

        UpdateButton(
            speedBtn,
            speedImg,
            speedText,
            speedPrice,
            speedLevel,
            upgradeManager.speedLevel,
            upgradeManager.speedMaxLevel,
            speedCost,
            economy.Coins >= speedCost
        );

        UpdateButton(
            lapBtn,
            lapImg,
            lapText,
            lapPrice,
            lapLevel,
            upgradeManager.lapMoneyLevel,
            upgradeManager.lapRewardMaxLevel,
            lapCost,
            economy.Coins >= lapCost
        );

        UpdateButton(
            driftBtn,
            driftImg,
            driftText,
            driftPrice,
            driftLevel,
            upgradeManager.driftMoneyLevel,
            upgradeManager.driftRewardMaxLevel,
            driftCost,
            economy.Coins >= driftCost
        );
    }

    private void UpdateButton(
        Button btn,
        Image img,
        TMPro.TMP_Text txt,
        TMPro.TMP_Text priceText,
        TMPro.TMP_Text levelText,
        int level,
        int maxLevel,
        int cost,
        bool canBuy)
    {
        if (levelText)
            levelText.text = $"level {level}/{maxLevel}";

        if (level >= maxLevel)
        {
            btn.interactable = false;

            SetAlpha(img, 0.3f);
            SetAlpha(txt, 0.3f);

            if (priceText)
                priceText.text = "MAX";

            return;
        }

        btn.interactable = canBuy;

        float a = canBuy ? 1f : 0.5f;

        SetAlpha(img, a);
        SetAlpha(txt, a);

        if (priceText)
            priceText.text = cost.ToString();
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

    private void SetAlpha(Graphic g, float a)
    {
        if (!g) return;

        Color c = g.color;
        c.a = a;
        g.color = c;
    }
}