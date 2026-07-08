using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [SerializeField] private CarController playerCar;

    [Header("Rewards")]
    public int coinsPerLap = 100;

    [SerializeField] private CoinPopup coinPopupPrefab;
    [SerializeField] private RectTransform popupParent;

    public int Coins => PlayerProfile.Coins;
    public System.Action<int, int> OnCoinsChanged;

    private float passiveIncomeTimer;

    private void Start()
    {
        if (playerCar != null)
            playerCar.OnLapCompleted += AwardLap;
    }

    void Update()
    {
        if (RaceManager.Instance == null)
            return;

        passiveIncomeTimer += Time.deltaTime;

        if (passiveIncomeTimer >= 3f)
        {
            passiveIncomeTimer = 0f;

            if (PlayerUpgrades.PassiveIncomeAmount > 0)
            {
                AddCoins(PlayerUpgrades.PassiveIncomeAmount);
            }
        }
    }

    private void OnDestroy()
    {
        if (playerCar != null)
            playerCar.OnLapCompleted -= AwardLap;
    }

    private void AwardLap()
    {
        int reward = Mathf.RoundToInt(
            coinsPerLap * PlayerUpgrades.LapMoneyMultiplier
        );

        AddCoins(reward);
        Debug.Log($"[Economy] Lap! +{reward} → total {Coins}");
    }

    public void AddCoins(int amount)
    {
        PlayerProfile.Coins += amount;

        OnCoinsChanged?.Invoke(
            PlayerProfile.Coins,
            amount
        );

        CoinNotifier.Notify(amount);

        SaveSystem.Save();

        if (coinPopupPrefab != null)
        {
            CoinPopup popup =
                Instantiate(
                    coinPopupPrefab,
                    popupParent
                );

            popup.transform.localPosition += new Vector3(
                Random.Range(-100f, 100f),
                Random.Range(-100f, 100f),
                0f
            );

            popup.Setup(amount);
        }
    }

    public bool SpendCoins(int amount)
    {
        if (Coins < amount)
            return false;

        PlayerProfile.Coins -= amount;

        OnCoinsChanged?.Invoke(
            PlayerProfile.Coins,
            -amount
        );

        CoinNotifier.Notify(-amount);

        SaveSystem.Save();

        return true;
    }
}

