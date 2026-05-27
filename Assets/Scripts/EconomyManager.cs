using UnityEngine;

/// <summary>
/// Handles the coin economy.
/// Listens to CarController2.OnLapCompleted and awards coinsPerLap.
/// No persistence between sessions – raw prototype.
/// </summary>
public class EconomyManager : MonoBehaviour
{
    [Header("Rewards")]
    public int coinsPerLap = 10;

    public int  Coins { get; private set; }
    public System.Action<int, int> OnCoinsChanged;   // (newTotal, delta)

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
        Coins += coinsPerLap;
        OnCoinsChanged?.Invoke(Coins, coinsPerLap);
        Debug.Log($"[Economy] Lap! +{coinsPerLap} → total {Coins}");
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        OnCoinsChanged?.Invoke(Coins, amount);
    }
}

