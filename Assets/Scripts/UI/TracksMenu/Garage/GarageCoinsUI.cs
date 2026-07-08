using UnityEngine;
using TMPro;

public class GarageCoinsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;

    private void OnEnable()
    {
        CoinNotifier.OnCoinsChanged += Refresh;

        Refresh(PlayerProfile.Coins, 0);
    }

    private void OnDisable()
    {
        CoinNotifier.OnCoinsChanged -= Refresh;
    }

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        coinsText.text = $"Money: {PlayerProfile.Coins}";
    }

    private void Refresh(int total, int delta)
    {
        coinsText.text = $"Money: {total}";
    }
}