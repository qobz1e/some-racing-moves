using TMPro;
using UnityEngine;

public class GarageCoinsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        coinsText.text = $"Money: {PlayerProfile.Coins}";
    }
}