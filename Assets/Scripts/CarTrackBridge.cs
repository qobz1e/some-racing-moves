using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Bridge component that lives on the Car GameObject.
/// It copies the TrackData reference to CarController at runtime,
/// so the car and the track can be separate GameObjects.
///
/// Attach this alongside CarController on the Car GO.
/// Assign trackGO in the Inspector (or via SceneBootstrap).
/// </summary>
public class CarTrackBridge : MonoBehaviour
{
    [Tooltip("The GameObject that has TrackData on it.")]
    public GameObject trackGO;

    private void Awake()
    {
        if (trackGO == null)
        {
            Debug.LogError("[CarTrackBridge] trackGO is not assigned!");
            return;
        }

        TrackData td = trackGO.GetComponent<TrackData>();
        if (td == null)
        {
            Debug.LogError("[CarTrackBridge] trackGO has no TrackData component!");
            return;
        }

        // Add CarController to this GO and wire up TrackData
        var ctrl = GetComponent<CarController2>();

        if (ctrl == null)
            ctrl = gameObject.AddComponent<CarController2>();

        ctrl.SetTrack(td);
    }
}

/// <summary>
/// Version of CarController that accepts an external TrackData reference.
/// Identical logic to CarController but decoupled from RequireComponent.
/// </summary>
public class CarController2 : MonoBehaviour
{
    [Header("Speed (units per second)")]
    public float maxSpeed     = 6f;
    public float acceleration = 2f;
    public float deceleration = 4f;

    [Header("Tap Control")]
    public float tapBoost = 2f;

    [Header("Waypoint following")]
    public float waypointReachDist = 0.4f;
    public float rotationSpeed     = 240f;

    public System.Action OnLapCompleted;
    public float CurrentSpeed { get; private set; }
    public float MaxSpeed     => maxSpeed;
    public int   LapCount     { get; private set; }
    public int   CurrentWaypoint { get; private set; }

    private TrackData _track;
    private bool      _hasStarted;
    private bool      _passedFirstWP;

    public void SetTrack(TrackData td)
    {
        _track = td;
        transform.position = _track.Get(0);
        CurrentWaypoint    = 0;
        Vector3 dir = _track.GetDirection(0);
        if (dir != Vector3.zero) transform.up = dir;
    }

    private void Update()
    {
        if (_track == null) return;

        if (WasTapped())
        {
            _hasStarted = true;

            CurrentSpeed += tapBoost;
            CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, maxSpeed);
        }

        // passive slowdown
        CurrentSpeed = Mathf.MoveTowards(
            CurrentSpeed,
            0f,
            deceleration * Time.deltaTime
        );

        if (!_hasStarted || CurrentSpeed <= 0f) return;

        float distThisFrame = CurrentSpeed * Time.deltaTime;

        Vector3 target = _track.Get(CurrentWaypoint + 1);

        Vector3 dir = (target - transform.position);
        float dist = dir.magnitude;

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
    }

    private void AdvanceWaypoint()
    {
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

        Vector3 dir = _track.GetDirection(CurrentWaypoint);
        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

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
