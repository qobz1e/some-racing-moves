using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [SerializeField] private UpgradeManager upgradeManager;

    [Header("Speed")]
    public float maxSpeed = 6f;
    public float deceleration = 4f;

    [Header("Tap Control")]
    public float tapBoost = 2f;

    [Header("Waypoint Following")]
    public float waypointReachDist = 0.5f;
    public float rotationSpeed = 360f;

    [Header("Drift Visual")]
    [SerializeField] private Transform visual;
    [SerializeField] private float driftVisualAngle = 45f;
    [SerializeField] private float driftVisualSmooth = 480f;

    public System.Action OnLapCompleted;

    public float CurrentSpeed { get; private set; }
    public float MaxSpeed => maxSpeed;

    public int LapCount { get; private set; }
    public int CurrentWaypoint { get; private set; }

    public bool IsTurning { get; private set; }
    private bool driftVisualActive;
    private float driftVisualAngleCurrent;

    public bool IsDrifting { get; set; }

    private TrackData _track;

    private bool _hasStarted;
    private bool _passedFirstWP;

    private float currentDriftAngle;
    private float driftDirection;
    private float driftPower = 1f;

    // ─────────────────────────────────────────────

    public void SetTrack(TrackData td)
    {
        _track = td;

        if (_track == null || _track.Count < 2)
            return;

        transform.position = _track.Get(0);

        CurrentWaypoint = 0;

        Vector3 dir = _track.GetDirection(0);

        if (dir != Vector3.zero)
            transform.up = dir;
    }

    // ─────────────────────────────────────────────

    private void Update()
    {
        if (_track == null || _track.Count < 2)
            return;

        HandleInput();
        HandleSpeed();

        if (!_hasStarted || CurrentSpeed <= 0f)
            return;

        MoveAlongTrack();
        UpdateDriftVisual();
    }

    // ─────────────────────────────────────────────

    private void HandleInput()
    {
        if (IsDrifting)
            return;

        if (WasTapped())
        {
            _hasStarted = true;

            CurrentSpeed += tapBoost;

            CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, maxSpeed * upgradeManager.speedMultiplier);
        }
    }

    // ─────────────────────────────────────────────

    private void HandleSpeed()
    {
        if (IsDrifting)
            return;

        CurrentSpeed = Mathf.MoveTowards(
            CurrentSpeed,
            0f,
            deceleration * Time.deltaTime
        );
    }

    // ─────────────────────────────────────────────

    private void MoveAlongTrack()
    {
        Vector3 target = _track.Get(CurrentWaypoint + 1);
        Vector3 dir = target - transform.position;
        float dist = dir.magnitude;

        Vector3 currentSegmentDir = _track.GetDirection(CurrentWaypoint);
        Vector3 nextSegmentDir = _track.GetDirection(CurrentWaypoint + 1);

        float cross = Vector3.Cross(currentSegmentDir, nextSegmentDir).z;

        driftDirection = Mathf.Sign(cross);

        float cornerAngle = Vector3.Angle(currentSegmentDir, nextSegmentDir);

        // ❗ НЕ используем IsTurning для дрифта теперь как триггер
        IsTurning = cornerAngle > 10f && dist < 2.0f;

        if (dist <= waypointReachDist)
        {
            AdvanceWaypoint();
            return;
        }

        dir.Normalize();

        transform.position += dir * CurrentSpeed * Time.deltaTime;

        float targetAngle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        float angle = Mathf.MoveTowardsAngle(
            transform.eulerAngles.z,
            targetAngle,
            rotationSpeed * Time.deltaTime
        );

        transform.eulerAngles = new Vector3(0, 0, angle);

        if (visual != null)
        {
            float driftOffset = 0f;

            if (IsDrifting && IsTurning)
                driftOffset = driftVisualAngle * driftDirection * driftPower;

            visual.localEulerAngles = new Vector3(0, 0, driftOffset);
        }
    }

    // ─────────────────────────────────────────────

    private void UpdateDriftVisual()
    {
        if (visual == null)
            return;

        float targetDrift =
            IsDrifting ? driftVisualAngle * driftDirection * driftPower : 0f;

        currentDriftAngle = Mathf.MoveTowards(
            currentDriftAngle,
            targetDrift,
            driftVisualSmooth * Time.deltaTime
        );

        visual.localEulerAngles = new Vector3(0, 0, currentDriftAngle);
    }

    private float GetDriftOffset()
    {
        if (!IsDrifting)
            return 0f;

        return driftVisualAngle * driftDirection * driftPower;
    }

    public void BeginDrift()
    {
        IsDrifting = true;
    }

    public void EndDrift()
    {
        IsDrifting = false;
    }

    public void SetDriftPower(int multiplier)
    {
        driftPower =
            Mathf.Clamp(1f + multiplier * 0.15f, 1f, 2f);
    }

    // ─────────────────────────────────────────────

    private void AdvanceWaypoint()
    {
        if (_track == null || _track.Count == 0)
            return;

        int next = (CurrentWaypoint + 1) % _track.Count;

        if (CurrentWaypoint == 0 && next == 1)
            _passedFirstWP = true;

        if (next == 0 && _passedFirstWP)
        {
            LapCount++;
            _passedFirstWP = false;
            OnLapCompleted?.Invoke();
        }

        CurrentWaypoint = next;
    }

    // ─────────────────────────────────────────────

    private static bool WasTapped()
    {
        bool keyboard =
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame;

        bool mouse =
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;

        bool touch =
            Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

        return keyboard || mouse || touch;
    }
}