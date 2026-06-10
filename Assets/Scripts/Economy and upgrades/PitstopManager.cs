using TMPro;
using UnityEngine;

public class PitstopManager : MonoBehaviour
{
    [SerializeField] private CarController car;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private TMP_Text warningText;

    public float serviceDistance;
    private bool servicing;
    private float serviceTimer;

    public float RemainingPercent =>
        Mathf.Clamp01(
            1f - car.TotalDistance / serviceDistance
    );

    private bool needsPitstop;

    void Awake()
    {
        serviceDistance = upgradeManager.durability;
    }   

    void Update()
    {
        if (!needsPitstop &&
            car.TotalDistance >= serviceDistance)
        {
            needsPitstop = true;

            warningText.gameObject.SetActive(true);
            warningText.text = "PITSTOP REQUIRED!";
        }

        if (servicing)
        {
            serviceTimer -= Time.deltaTime;

            warningText.text =
                $"PITSTOP {Mathf.CeilToInt(serviceTimer)}";

            if (serviceTimer <= 0f)
            {
                servicing = false;

                PerformPitstop();
            }

            return;
        }
    }

    public float SpeedMultiplier =>
        needsPitstop ? 0.2f : 1f;

    public bool NeedsPitstop => needsPitstop;

    public void PerformPitstop()
    {
        needsPitstop = false;

        serviceDistance = upgradeManager.durability;
        car.ResetDistance();

        warningText.gameObject.SetActive(false);
    }

    public void StartService(float duration)
    {
        servicing = true;
        serviceTimer = duration;
    }
}