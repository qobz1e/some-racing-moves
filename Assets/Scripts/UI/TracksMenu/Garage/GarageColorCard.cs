using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GarageColorCard : MonoBehaviour
{
    [SerializeField] private GarageColorManager manager;

    [SerializeField] private int colorIndex;

    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text buttonText;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        icon.sprite =
            manager.CurrentCar.prefabs[colorIndex]
            .GetComponentInChildren<SpriteRenderer>()
            .sprite;

        if (!manager.IsOwned(colorIndex))
        {
            buttonText.text = "500";
        }
        else
        {
            buttonText.text =
                manager.IsSelected(colorIndex)
                    ? "Current"
                    : "Owned";
        }
    }

    public void OnButtonPressed()
    {
        if (!manager.IsOwned(colorIndex))
        {
            if (manager.BuyColor(colorIndex))
                manager.SelectColor(colorIndex);
        }
        else
        {
            manager.SelectColor(colorIndex);
        }

        manager.RefreshAllCards();
    }
}