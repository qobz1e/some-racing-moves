using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GarageCarCard : MonoBehaviour
{
    [SerializeField] private GarageManager garage;

    [SerializeField] private int carIndex;

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text title;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text buttonText;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        CarData car = garage.Cars[carIndex];

        GameObject prefab =
            car.prefabs[
                PlayerProfile.Colors[carIndex].CurrentColor
            ];

        SpriteRenderer sr =
            prefab.GetComponentInChildren<SpriteRenderer>();

        title.text = car.carName;

        if (!garage.IsOwned(carIndex))
        {
            buttonText.text = $"Buy ({car.price})";
            icon.sprite = car.icon;
        }
        else if (garage.IsSelected(carIndex))
        {
            buttonText.text = "Chosen";
            icon.sprite = sr.sprite;
        }
        else
        {
            buttonText.text = "Owned";
            icon.sprite = sr.sprite;
        }
    }

    public void OnButtonPressed()
    {
        if (!garage.IsOwned(carIndex))
        {
            if (!garage.BuyCar(carIndex))
                return;
        }

        garage.SelectCar(carIndex);
        garage.RefreshAllCards();
    }
}