using UnityEngine;

public class GarageManager : MonoBehaviour
{
    [SerializeField] private CarData[] cars;
    [SerializeField] private GarageCarCard[] cards;

    [SerializeField] private EconomyManager economy;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private UpgradeHUD upgradeHUD;
    [SerializeField] private GarageColorManager colorManager;

    public CarData[] Cars => cars;

    public bool IsOwned(int index)
    {
        return PlayerProfile.OwnedCars[index];
    }

    public bool IsSelected(int index)
    {
        return PlayerProfile.CurrentCarLevel == index;
    }

    public bool BuyCar(int index)
    {
        CarData car = cars[index];

        if (PlayerProfile.OwnedCars[index])
            return true;

        if (PlayerProfile.Coins < car.price)
            return false;

        PlayerProfile.Coins -= car.price;
        upgradeManager.Refresh();

        PlayerProfile.OwnedCars[index] = true;

        SaveSystem.Save();

        return true;
    }

    public void SelectCar(int index)
    {
        if (!PlayerProfile.OwnedCars[index])
            return;

        PlayerProfile.CurrentCarLevel = index;

        SaveSystem.Save();

        upgradeManager.Refresh();
        upgradeHUD.Refresh();
        colorManager.RefreshAllCards();
    }

    public void RefreshAllCards()
    {
        foreach (GarageCarCard card in cards)
            card.Refresh();
    }
}