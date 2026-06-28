using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [SerializeField] private CarController playerCar;

    [Header("Rewards")]
    public int coinsPerLap = 100;

    [SerializeField] private CoinPopup coinPopupPrefab;
    [SerializeField] private RectTransform popupParent;

    public int  Coins { get; private set; }
    public System.Action<int, int> OnCoinsChanged;

    private float passiveIncomeTimer;

    private void Start()
    {
        if (playerCar != null)
            playerCar.OnLapCompleted += AwardLap;

        Coins = PlayerProfile.Coins;
    }

    void Update()
    {
        passiveIncomeTimer += Time.deltaTime;

        if (passiveIncomeTimer >= 3f)
        {
            passiveIncomeTimer = 0f;

            if (PlayerProfile.PassiveIncomeAmount > 0)
            {
                AddCoins(PlayerProfile.PassiveIncomeAmount);
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
            coinsPerLap * PlayerProfile.LapMoneyMultiplier
        );

        AddCoins(reward);
        Debug.Log($"[Economy] Lap! +{reward} → total {Coins}");
    }

    public void AddCoins(int amount)
    {
        Coins += amount;

        OnCoinsChanged?.Invoke(Coins, amount);

        PlayerProfile.Coins = Coins;

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
}

