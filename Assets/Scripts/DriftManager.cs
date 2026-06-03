using TMPro;
using UnityEngine;

public class DriftManager : MonoBehaviour
{
    [SerializeField] private CarController car;
    [SerializeField] private EconomyManager economy;
    [SerializeField] private UpgradeManager upgrades;

    [Header("UI")]
    [SerializeField] private TMP_Text driftText;

    [Header("Reward")]
    public float rewardInterval = 1f;
    public int baseReward = 5;

    [Header("Combo")]
    public float comboDecayTime = 1.2f;

    private float rewardTimer;
    private float comboTimer;

    private int combo = 0;
    private bool wasDrifting;

    void Update()
    {
        if (car == null) return;

        HandleDrift();
        UpdateUI();
    }

    void HandleDrift()
    {
        if (car.IsDrifting)
        {
            // старт дрифта
            if (!wasDrifting)
            {
                combo = 1;
                rewardTimer = rewardInterval;
                comboTimer = comboDecayTime;
            }

            // таймер награды
            rewardTimer -= Time.deltaTime;

            if (rewardTimer <= 0f)
            {
                rewardTimer = rewardInterval;

                int reward =
                    Mathf.RoundToInt(
                        baseReward *
                        combo *
                        upgrades.driftMoneyMultiplier
                    );

                economy.AddCoins(reward);

                combo++;
                comboTimer = comboDecayTime;
            }

            // удержание комбо
            comboTimer = comboDecayTime;
        }
        else
        {
            comboTimer -= Time.deltaTime;

            if (comboTimer <= 0f)
            {
                combo = 0;
            }
        }

        wasDrifting = car.IsDrifting;
    }

    void UpdateUI()
    {
        if (!driftText) return;

        if (!car.IsDrifting && combo <= 0)
        {
            driftText.gameObject.SetActive(false);
            return;
        }

        driftText.gameObject.SetActive(true);

        driftText.text =
            $"DRIFT x{combo}\n" +
            $"{Mathf.Abs(car.DriftAngle):0}°";
    }
}