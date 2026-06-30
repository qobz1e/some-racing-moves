using UnityEngine;

public class GarageManager : MonoBehaviour
{
    [SerializeField] private CarData[] cars;
    [SerializeField] private GarageCarCard[] cards;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private UpgradeHUD upgradeHUD;

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
    }

    public void RefreshAllCards()
    {
        foreach (GarageCarCard card in cards)
            card.Refresh();
    }
}