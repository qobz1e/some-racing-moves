using UnityEngine;
using UnityEngine.UI;

public class GarageUI : MonoBehaviour
{
    [SerializeField] private Image carImageTracksMenu;
    [SerializeField] private Image carImageGarage;
    [SerializeField] private GarageManager garage;

    [SerializeField] private GameObject tracksMenu;
    [SerializeField] private GameObject garageMenu;
    [SerializeField] private GameObject carsShop;

    void OnEnable()
    {
        RefreshImage();
    }

    public void OpenGarage()
    {
        tracksMenu.SetActive(false);
        carsShop.SetActive(false);
        garageMenu.SetActive(true);
        RefreshImage();
    }

    public void OpenTracksMenu()
    {
        garageMenu.SetActive(false);
        carsShop.SetActive(false);
        tracksMenu.SetActive(true);
        RefreshImage();
    }

    public void OpenCarsShop()
    {
        tracksMenu.SetActive(false);
        garageMenu.SetActive(false);
        carsShop.SetActive(true);
    }

    private void RefreshImage()
    {
        GameObject prefab =
            garage.Cars[
                PlayerProfile.CurrentCarLevel
            ].prefabs[
                PlayerProfile.CurrentCarColor
            ];

        Sprite sprite =
            prefab.GetComponent<SpriteRenderer>().sprite;

        carImageTracksMenu.sprite = sprite;

        carImageGarage.sprite = sprite;
    }
}