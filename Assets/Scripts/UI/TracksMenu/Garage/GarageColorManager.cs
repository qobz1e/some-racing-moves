using UnityEngine;

public class GarageColorManager : MonoBehaviour
{
    [SerializeField] private EconomyManager economy;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private CarDatabase database;
    [SerializeField] private GarageColorCard[] cards;

    public CarData CurrentCar =>
        database.cars[PlayerProfile.CurrentCarLevel];

    public bool IsOwned(int color)
    {
        return PlayerProfile
            .Colors[PlayerProfile.CurrentCarLevel]
            .OwnedColors[color];
    }

    public bool IsSelected(int color)
    {
        return PlayerProfile
            .Colors[PlayerProfile.CurrentCarLevel]
            .CurrentColor == color;
    }

    public bool BuyColor(int color)
    {
        const int price = 500;

        if (IsOwned(color))
            return true;

        if (PlayerProfile.Coins < price)
            return false;

        if (!economy.SpendCoins(price))
            return false;

        upgradeManager.Refresh();

        PlayerProfile
            .Colors[PlayerProfile.CurrentCarLevel]
            .OwnedColors[color] = true;

        SaveSystem.Save();

        return true;
    }

    public void SelectColor(int color)
    {
        if (!IsOwned(color))
            return;

        PlayerProfile
            .Colors[PlayerProfile.CurrentCarLevel]
            .CurrentColor = color;

        SaveSystem.Save();
    }

    public void RefreshAllCards()
    {
        foreach (var c in cards)
            c.Refresh();
    }
}