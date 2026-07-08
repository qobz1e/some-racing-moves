using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatBar : MonoBehaviour
{
    public enum PlayerStatType
    {
        LapMoney,
        DriftMoney,
        PassiveIncome
    }

    [Header("Setup")]
    [SerializeField] private PlayerStatType stat;

    [SerializeField] private UpgradeManager upgrades;
    [SerializeField] private EconomyManager economy;

    [Header("UI")]
    [SerializeField] private StatCellStart startCell;
    [SerializeField] private Transform cellsParent;
    [SerializeField] private Image cellPrefab;

    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite upgradeSprite;

    const int CELL_COUNT = 5;

    readonly List<Image> cells = new();


    void Awake()
    {
        startCell.SetTitle(GetTitle());

        Build();

        startCell.Button.onClick.AddListener(BuyUpgrade);
    }


    void Start()
    {
        Refresh();
    }


    void OnEnable()
    {
        upgrades.OnUpgradeChanged += Refresh;
        economy.OnCoinsChanged += OnCoinsChanged;
    }


    void OnDisable()
    {
        upgrades.OnUpgradeChanged -= Refresh;
        economy.OnCoinsChanged -= OnCoinsChanged;
    }


    void Build()
    {
        foreach (Transform c in cellsParent)
            Destroy(c.gameObject);

        cells.Clear();


        for (int i = 0; i < CELL_COUNT; i++)
        {
            Image img =
                Instantiate(cellPrefab, cellsParent);

            img.enabled = true;
            img.gameObject.SetActive(true);

            cells.Add(img);
        }
    }


    public void Refresh()
    {
        int level = GetLevel();


        for (int i = 0; i < CELL_COUNT; i++)
        {
            cells[i].enabled = true;

            if (i < level)
                cells[i].sprite = upgradeSprite;
            else
                cells[i].sprite = emptySprite;
        }


        RefreshStartCell();
    }


    int GetLevel()
    {
        switch (stat)
        {
            case PlayerStatType.LapMoney:
                return PlayerUpgrades.LapMoneyLevel;

            case PlayerStatType.DriftMoney:
                return PlayerUpgrades.DriftMoneyLevel;

            case PlayerStatType.PassiveIncome:
                return PlayerUpgrades.PassiveIncomeLevel;
        }

        return 0;
    }


    int GetCost()
    {
        switch (stat)
        {
            case PlayerStatType.LapMoney:
                return upgrades.GetLapMoneyCost();

            case PlayerStatType.DriftMoney:
                return upgrades.GetDriftMoneyCost();

            case PlayerStatType.PassiveIncome:
                return upgrades.GetPassiveIncomeCost();
        }

        return 0;
    }


    void BuyUpgrade()
    {
        switch (stat)
        {
            case PlayerStatType.LapMoney:
                upgrades.BuyLapMoneyUpgrade();
                break;

            case PlayerStatType.DriftMoney:
                upgrades.BuyDriftMoneyUpgrade();
                break;

            case PlayerStatType.PassiveIncome:
                upgrades.BuyPassiveIncomeUpgrade();
                break;
        }

        Refresh();
    }


    void RefreshStartCell()
    {
        int level = GetLevel();

        int maxLevel = CELL_COUNT;

        int cost = GetCost();


        startCell.LevelText.text =
            $"{level}/{maxLevel}";


        if (level >= maxLevel)
        {
            startCell.PriceText.text = "MAX";

            startCell.Button.interactable = false;

            SetAlpha(startCell.Icon, 0.3f);

            return;
        }


        startCell.PriceText.text =
            cost.ToString();


        bool canBuy =
            economy.Coins >= cost;


        startCell.Button.interactable =
            canBuy;


        SetAlpha(
            startCell.Icon,
            canBuy ? 1f : 0.5f
        );
    }


    string GetTitle()
    {
        switch (stat)
        {
            case PlayerStatType.LapMoney:
                return "LAP MONEY";

            case PlayerStatType.DriftMoney:
                return "DRIFT MONEY";

            case PlayerStatType.PassiveIncome:
                return "PASSIVE INCOME";
        }

        return "";
    }


    void SetAlpha(Graphic g, float a)
    {
        if (!g)
            return;

        Color c = g.color;
        c.a = a;
        g.color = c;
    }


    void OnCoinsChanged(int total, int delta)
    {
        Refresh();
    }
}