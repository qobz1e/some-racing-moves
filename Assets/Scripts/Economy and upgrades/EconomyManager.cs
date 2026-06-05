using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [SerializeField] private UpgradeManager upgradeManager;

    [Header("Rewards")]
    public int coinsPerLap = 100;

    [SerializeField] private CoinPopup coinPopupPrefab;
    [SerializeField] private RectTransform popupParent;

    public int  Coins { get; private set; }
    public System.Action<int, int> OnCoinsChanged;

    private CarController _car;

    private void Awake()
    {
        _car = FindAnyObjectByType<CarController>();
        if (_car != null)
            _car.OnLapCompleted += AwardLap;
        else
            Debug.LogWarning("[EconomyManager] CarController not found.");
    }

    private void OnDestroy()
    {
        if (_car != null)
            _car.OnLapCompleted -= AwardLap;
    }

    private void AwardLap()
    {
        int reward = Mathf.RoundToInt(
            coinsPerLap * upgradeManager.lapMoneyMultiplier
        ); 

        AddCoins(reward);
        Debug.Log($"[Economy] Lap! +{reward} → total {Coins}");
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        OnCoinsChanged?.Invoke(Coins, amount);

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

