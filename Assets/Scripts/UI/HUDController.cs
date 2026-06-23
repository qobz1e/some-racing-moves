using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private CarController _car;
    [SerializeField] private EconomyManager _eco;
    [SerializeField] private RaceManager raceManager;

    [Header("Text Labels (TMPro)")]
    public TMP_Text txtCoins;
    public TMP_Text txtLap;
    public TMP_Text txtSpeed;
    public TMP_Text txtHint;
    public TMP_Text txtCoinPopup;

    [Header("Race")]
    public TMP_Text txtRaceTime;

    [Header("Speed bar")]
    public Slider speedBar;

    [Header("Nitro")]
    public TMP_Text txtNitro;
    public Slider nitroBar;

    [Header("Coin popup")]
    public float popupDuration = 1.2f;

    // ── Private refs ──────────────────────────────────────────────────────────
    private float _displayedNitroValue = 1f;
    private float _popupTimer;
    private bool  _hintShown = true;
    private float raceTime;

    private void Start()
    {
        if (_eco)
            _eco.OnCoinsChanged += OnCoinsChanged;

        if (_car)
            _car.OnLapCompleted += OnLapCompleted;

        // Initial state
        SetCoins(0);
        SetLap(0);
        if (txtHint)      txtHint.text = "W TO ACCELERATE\nA/D TO STEER\nLEFT SHIFT TO USE NITRO";
        if (txtCoinPopup) txtCoinPopup.gameObject.SetActive(false);
        if (speedBar)     speedBar.value = 0f;
        if (nitroBar)     nitroBar.value = 0f;
    }

    private void OnDestroy()
    {
        if (_eco != null) _eco.OnCoinsChanged  -= OnCoinsChanged;
        if (_car != null) _car.OnLapCompleted  -= OnLapCompleted;
    }

    private void Update()
    {
        if (_car == null) return;

        if (raceManager && txtRaceTime)
        {
            txtRaceTime.text =
                FormatTime(raceManager.CurrentRaceTime);
        }

        // Speed
        float ratio = _car.MaxSpeed > 0 ? _car.CurrentSpeed / _car.MaxSpeed : 0f;
        if (speedBar) speedBar.value = ratio;
        if (txtSpeed) txtSpeed.text  = $"{_car.CurrentSpeed * 10f:F0} km/h";

        // Nitro 
        bool hasNitro =
            _car != null &&
            _car.HasNitro;

        if (nitroBar)
            nitroBar.gameObject.SetActive(hasNitro);

        if (txtNitro)
            txtNitro.gameObject.SetActive(hasNitro);

        if (hasNitro)
        {
            float targetNitro =
                Mathf.Clamp01(
                    _car.NitroAmount /
                    _car.MaxNitro
                );

            _displayedNitroValue = Mathf.Lerp(
                _displayedNitroValue,
                targetNitro,
                6f * Time.deltaTime
            );

            nitroBar.value = _displayedNitroValue;

            if (_car.NitroAmount <= 0 && _car.NitroCooldown > 0f)
            {
                txtNitro.text =
                    $"Cooldown {_car.NitroCooldown:F1} s";
            }
            else
            {
                txtNitro.text =
                    $"{_car.NitroAmount:F1} s";
            }
        }

        // Hide hint after first movement
        if (_hintShown && _car.CurrentSpeed > 0.1f)
        {
            if (txtHint) txtHint.gameObject.SetActive(false);
            _hintShown = false;
        }

        // Coin popup countdown
        if (_popupTimer > 0f)
        {
            _popupTimer -= Time.deltaTime;
            if (_popupTimer <= 0f && txtCoinPopup)
                txtCoinPopup.gameObject.SetActive(false);
        }
    }

    // ── Callbacks ─────────────────────────────────────────────────────────────
    private void OnCoinsChanged(int total, int delta)
    {
        SetCoins(total);
        ShowCoinPopup(delta);
    }

    private void OnLapCompleted()
    {
        if (_car != null) SetLap(_car.LapCount);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private void SetCoins(int amount)
    {
        if (txtCoins) txtCoins.text = $"Money: {amount}";
    }

    private void SetLap(int lap)
    {
        if (txtLap) txtLap.text = $"Lap: {lap}";
    }

    private void ShowCoinPopup(int delta)
    {
        if (!txtCoinPopup) return;

        if (delta > 0)
            txtCoinPopup.text = $"+{delta}";
        else
            txtCoinPopup.text = $"{delta}";

        txtCoinPopup.gameObject.SetActive(true);
        _popupTimer = popupDuration;
    }

    string FormatTime(float t)
    {
        int minutes =
            Mathf.FloorToInt(t / 60f);

        int seconds =
            Mathf.FloorToInt(t % 60f);

        int milliseconds =
            Mathf.FloorToInt(
                (t * 1000f) % 1000f
            );

        return
            $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}

