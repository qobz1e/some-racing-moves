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
    [SerializeField] private GameObject colorsMenu;

    [SerializeField] private Sprite unknownCarSprite;

    private void OnEnable()
    {
        RefreshImages();
    }

    public void OpenGarage()
    {
        tracksMenu.SetActive(false);
        carsShop.SetActive(false);
        colorsMenu.SetActive(false);
        garageMenu.SetActive(true);

        RefreshImages();
    }

    public void OpenTracksMenu()
    {
        garageMenu.SetActive(false);
        carsShop.SetActive(false);
        colorsMenu.SetActive(false);
        tracksMenu.SetActive(true);

        RefreshImages();
    }

    public void OpenCarsShop()
    {
        tracksMenu.SetActive(false);
        garageMenu.SetActive(false);
        colorsMenu.SetActive(false);
        carsShop.SetActive(true);

        RefreshImages();
    }

    public void OpenColorsMenu()
    {
        tracksMenu.SetActive(false);
        garageMenu.SetActive(false);
        carsShop.SetActive(false);
        colorsMenu.SetActive(true);

        RefreshImages();
    }

    private void RefreshImages()
    {
        if (!PlayerProfile.TutorialCompleted &&
            PlayerProfile.TutorialStep < (int)TutorialStep.ChooseCar)
        {
            carImageTracksMenu.sprite = unknownCarSprite;
            carImageGarage.sprite = unknownCarSprite;

            return;
        }

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

        garage.RefreshAllCards();
    }
}