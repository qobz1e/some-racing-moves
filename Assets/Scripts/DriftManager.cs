using UnityEngine;
using UnityEngine.InputSystem;

public class DriftManager : MonoBehaviour
{
    [SerializeField] private CarController car;
    [SerializeField] private DriftUI ui;
    [SerializeField] private EconomyManager economy;
    [SerializeField] private UpgradeManager upgradeManager;

    [Header("Rewards")]
    public int baseReward = 10;

    [Header("Slow Motion")]
    public float slowMo = 0.3f;

    bool drifting;
    bool driftLocked;

    int multiplier = 0;
    float cooldown;

    private enum DriftEndReason
    {
        None,
        BadTap,
        FullRotation,
        ZoneEnd
    }

    private DriftEndReason endReason;

    void Update()
    {
        if (!car) return;

        HandleLockReset();
        HandleState();

        if (drifting)
            TickDrift();
    }

    void HandleLockReset()
    {
        if (!car.IsTurning && driftLocked)
        {
            driftLocked = false;
            endReason = DriftEndReason.None;
        }
    }

    void HandleState()
    {
        if (driftLocked) return;

        if (car.IsTurning && car.CurrentSpeed > car.maxSpeed * upgradeManager.speedMultiplier * 0.4f)
        {
            cooldown = 0.2f;

            if (!drifting)
                StartDrift();
        }
        else
        {
            cooldown -= Time.unscaledDeltaTime;

            if (cooldown <= 0f && drifting)
                EndDrift(DriftEndReason.ZoneEnd);
        }
    }

    void StartDrift()
    {
        drifting = true;
        multiplier = 0;

        Debug.Log("DRIFT START");

        car.BeginDrift();

        ui.Show();
        ui.SetMultiplier(multiplier);

        Time.timeScale = slowMo;
        Time.fixedDeltaTime = 0.02f * slowMo;
    }

    void TickDrift()
    {
        ui.RotateNeedle();

        if (ui.DidFullRotation)
        {
            Debug.Log("DRIFT FAIL (full rotation)");
            EndDrift(DriftEndReason.FullRotation);
            return;
        }

        if (WasTapped())
        {
            if (ui.IsInSuccessZone())
            {
                multiplier++;

                int reward = baseReward * multiplier;

                reward = Mathf.RoundToInt(
                    reward * upgradeManager.driftMoneyMultiplier
                ); 
                
                economy.AddCoins(reward);

                Debug.Log("DRIFT + " + reward);

                ui.SetMultiplier(multiplier);

                car.SetDriftPower(multiplier);

                ui.ResetRotation();
            }
            else
            {
                Debug.Log("DRIFT FAIL (bad tap)");
                EndDrift(DriftEndReason.BadTap);
            }
        }
    }

    void EndDrift(DriftEndReason reason)
    {
        drifting = false;

        driftLocked = true;
        endReason = reason;

        Debug.Log("DRIFT END: " + reason);

        string failMessage = ""; 

        switch (reason) 
        { 
            case DriftEndReason.BadTap: 
                failMessage = "WRONG PLACE TAP"; 
                break; 

            case DriftEndReason.FullRotation: 
                failMessage = "MISSED SUCCESS ZONE"; 
                break;
        }

        if (reason != DriftEndReason.ZoneEnd)
        {
            ui.ShowFailOnly(failMessage);

            Invoke(nameof(HideDriftUI), 0.5f);
        }
        else
        {
            ui.Hide();
        }

        car.EndDrift();

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    void HideDriftUI()
    {
        ui.Hide();
    }

    static bool WasTapped()
    {
        return Keyboard.current?.spaceKey.wasPressedThisFrame == true ||
               Mouse.current?.leftButton.wasPressedThisFrame == true ||
               Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true;
    }
}
