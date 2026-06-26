using UnityEngine;

public class GarageUI : MonoBehaviour
{
    [SerializeField] private GameObject tracksMenu;
    [SerializeField] private GameObject garageMenu;

    public void OpenGarage()
    {
        tracksMenu.SetActive(false);
        garageMenu.SetActive(true);
    }

    public void CloseGarage()
    {
        garageMenu.SetActive(false);
        tracksMenu.SetActive(true);
    }
}