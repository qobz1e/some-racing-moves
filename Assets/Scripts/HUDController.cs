using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Drives the on-screen HUD.
///
/// Required Canvas children (assign in Inspector or let FindObjectOfType handle it):
///   txtCoins     – TMP label, e.g. "Money: 0"
///   txtLap       – TMP label, e.g. "Lap: 0"
///   txtSpeed     – TMP label, e.g. "0 km/h"
///   txtHint      – TMP label "TAP TO START" / disappears after first tap
///   coinPopup    – TMP label for floating "+10" flash
///   speedbar     – UnityEngine.UI.Slider (0–1, no interaction)
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("Text Labels (TMPro)")]
    public TMP_Text txtCoins;
    public TMP_Text txtLap;
    public TMP_Text txtSpeed;
    public TMP_Text txtHint;
    public TMP_Text txtCoinPopup;

    [Header("Speed bar")]
    public Slider speedBar;

    [Header("Coin popup")]
    public float popupDuration = 1.2f;

    // ── Private refs ──────────────────────────────────────────────────────────
    private CarController _car;
    private EconomyManager _eco;
    private float          _popupTimer;
    private bool           _hintShown = true;

    private void Start()
    {
        _car = FindAnyObjectByType<CarController>();
        _eco = FindAnyObjectByType<EconomyManager>();

        if (_eco != null)
            _eco.OnCoinsChanged += OnCoinsChanged;

        if (_car != null)
            _car.OnLapCompleted += OnLapCompleted;

        // Initial state
        SetCoins(0);
        SetLap(0);
        if (txtHint)      txtHint.text = "TAP TO START";
        if (txtCoinPopup) txtCoinPopup.gameObject.SetActive(false);
        if (speedBar)     speedBar.value = 0f;
    }

    private void OnDestroy()
    {
        if (_eco != null) _eco.OnCoinsChanged  -= OnCoinsChanged;
        if (_car != null) _car.OnLapCompleted  -= OnLapCompleted;
    }

    private void Update()
    {
        if (_car == null) return;

        // Speed
        float ratio = _car.MaxSpeed > 0 ? _car.CurrentSpeed / _car.MaxSpeed : 0f;
        if (speedBar) speedBar.value = ratio;
        if (txtSpeed) txtSpeed.text  = $"{_car.CurrentSpeed * 10f:F0} km/h";

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
        txtCoinPopup.text = $"+{delta}";
        txtCoinPopup.gameObject.SetActive(true);
        _popupTimer = popupDuration;
    }
}

