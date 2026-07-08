using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private StatType stat;

    [SerializeField] private GarageManager garage;
    [SerializeField] private UpgradeManager upgrades;
    [SerializeField] private EconomyManager economy;

    [Header("UI")]
    [SerializeField] private StatCellStart startCell;
    [SerializeField] private Transform cellsParent;
    [SerializeField] private Image cellPrefab;

    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite baseSprite;
    [SerializeField] private Sprite upgradeSprite;

    const int BASE_CELL_COUNT = 10;
    const int UPGRADE_CELL_COUNT = 5;
    const int CELL_COUNT = 15;

    readonly List<Image> cells = new();

    void Awake()
    {
        startCell.SetTitle(GetTitle());

        Build();

        startCell.Button.onClick.AddListener(BuyUpgrade);
    }

    void Start()
    {
        garage.OnSelectedCarChanged += Refresh;
        upgrades.OnUpgradeChanged += Refresh;
        CoinNotifier.OnCoinsChanged += OnCoinsChanged;

        Refresh();
    }

    void OnDestroy()
    {
        garage.OnSelectedCarChanged -= Refresh;
        upgrades.OnUpgradeChanged -= Refresh;
        CoinNotifier.OnCoinsChanged -= OnCoinsChanged;
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

            cells.Add(img);
        }
    }

    public void Refresh()
    {
        int baseCells = GetBaseCells();
        int upgradeCells = GetUpgradeLevel();

        int totalFilled = baseCells + upgradeCells;

        for (int i = 0; i < CELL_COUNT; i++)
        {
            if (i < baseCells)
                cells[i].sprite = baseSprite;
            else if (i < totalFilled)
                cells[i].sprite = upgradeSprite;
            else
                cells[i].sprite = emptySprite;
        }

        RefreshStartCell();
    }

    int GetBaseCells()
    {
        CarStats s =
            garage.Cars[
                PlayerProfile.CurrentCarLevel
            ].stats;

        switch (stat)
        {
            // 6..10 -> 6..10
            case StatType.Engine:
                return Mathf.RoundToInt(s.maxSpeed);

            // 10..14 -> 6..10
            case StatType.Turbo:
                return Scale(s.acceleration, 10f, 14f, 6, 10);

            // 0.5..2.5 -> 4..10
            case StatType.Tires:
                return Scale(s.traction, 0.5f, 2.5f, 4, 10);

            // always 5
            case StatType.Aerodynamics:
                return 5;

            case StatType.Nitro:
                return 0;
        }

        return 0;
    }

    int GetUpgradeLevel()
    {
        switch (stat)
        {
            case StatType.Engine:
                return PlayerProfile.Current.EngineLevel;

            case StatType.Turbo:
                return PlayerProfile.Current.TurboLevel;

            case StatType.Tires:
                return PlayerProfile.Current.TiresLevel;

            case StatType.Aerodynamics:
                return PlayerProfile.Current.AerodynamicsLevel;

            case StatType.Nitro:
                return PlayerProfile.Current.NitroLevel;
        }

        return 0;
    }

    int Scale(
        float value,
        float minValue,
        float maxValue,
        int minCells,
        int maxCells)
    {
        return Mathf.RoundToInt(
            Mathf.Lerp(
                minCells,
                maxCells,
                Mathf.InverseLerp(
                    minValue,
                    maxValue,
                    value
                )
            )
        );
    }

    void RefreshStartCell()
    {
        int level = GetUpgradeLevel();
        int maxLevel = 5;
        int cost = GetUpgradeCost();

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

        startCell.Button.interactable = canBuy;

        SetAlpha(startCell.Icon,
            canBuy ? 1f : .5f);
    }

    void BuyUpgrade()
    {
        switch (stat)
        {
            case StatType.Engine:
                upgrades.BuyEngineUpgrade();
                break;

            case StatType.Turbo:
                upgrades.BuyTurboUpgrade();
                break;

            case StatType.Tires:
                upgrades.BuyTiresUpgrade();
                break;

            case StatType.Aerodynamics:
                upgrades.BuyAerodynamicsUpgrade();
                break;

            case StatType.Nitro:
                upgrades.BuyNitroUpgrade();
                break;
        }

        Refresh();
    }

    int GetUpgradeCost()
    {
        switch (stat)
        {
            case StatType.Engine:
                return upgrades.GetEngineCost();

            case StatType.Turbo:
                return upgrades.GetTurboCost();

            case StatType.Tires:
                return upgrades.GetTiresCost();

            case StatType.Aerodynamics:
                return upgrades.GetAerodynamicsCost();

            case StatType.Nitro:
                return upgrades.GetNitroCost();
        }

        return 0;
    }

    void SetAlpha(Graphic g, float a)
    {
        if (!g)
            return;

        Color c = g.color;
        c.a = a;
        g.color = c;
    }

    string GetTitle()
    {
        switch (stat)
        {
            case StatType.Engine: return "ENGINE";
            case StatType.Turbo: return "ACCEL";
            case StatType.Tires: return "TIRES";
            case StatType.Aerodynamics: return "AERO";
            case StatType.Nitro: return "NITRO";
        }

        return "";
    }

    void OnCoinsChanged(int total, int delta)
    {
        Refresh();
    }
}