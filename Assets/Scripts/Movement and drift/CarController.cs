using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [SerializeField] private UpgradeManager upgradeManager;

    [Header("Movement")]
    public float acceleration = 15f;
    public float maxSpeed = 6f;
    public float drag = 2f;

    [SerializeField] private float wallBounce = 1f;

    private Vector2 moveForce;
    float Accel => acceleration * upgradeManager.speedMultiplier;
    public float CurrentSpeed => moveForce.magnitude;
    public float MaxSpeed => maxSpeed * upgradeManager.speedMultiplier;

    [Header("Steering")]
    public float steerAngle = 30f;

    [Header("Drift")]
    public float traction = 0.1f;
    [SerializeField] private TrailRenderer leftTrail;
    [SerializeField] private TrailRenderer rightTrail;

    public bool IsDrifting { get; private set; }
    public float DriftAngle { get; private set; }
    private float driftGraceTimer;

    //private float brakeFactor;

    public int LapCount { get; private set; }
    public bool checkpointPassed { get; private set; }
    public System.Action OnLapCompleted;

    private int roadContacts = 0;
    private bool isOnRoad;
    float offRoadSpeedLimit = 3f;

    void Update()
    {
        //brakeFactor = Keyboard.current.spaceKey.isPressed ? 1f : 0f;

        HandleAcceleration();
        HandleSteering();
        HandleDrag(); 
        HandleDriftPhysics();
        Move();

        void UpdateSkidMarks()
        {
            bool showSkid =
                IsDrifting && moveForce.magnitude > 3f;

            if (leftTrail != null)
                leftTrail.emitting = showSkid;

            if (rightTrail != null)
                rightTrail.emitting = showSkid;
        }

        UpdateSkidMarks();
    }

    // ─────────────────────────────

    void HandleAcceleration()
    {
        float accel = Accel;

        if (!IsOnRoad())
            accel *= 0.9f;

        if (Keyboard.current.wKey.isPressed)
        {
            moveForce += (Vector2)transform.up * accel * Time.deltaTime;
        }

        float currentLimit = IsOnRoad() ? MaxSpeed : offRoadSpeedLimit;

        float speed = moveForce.magnitude;

        if (speed > currentLimit)
        {
            moveForce = moveForce.normalized *
                Mathf.MoveTowards(speed, currentLimit, 6f * Time.deltaTime);
        }
    }

    // ─────────────────────────────

    void HandleSteering()
    {
        float steerInput = 0f;

        if (Keyboard.current.aKey.isPressed)
            steerInput = 1f;

        if (Keyboard.current.dKey.isPressed)
            steerInput = -1f;

        transform.Rotate(
            Vector3.forward,
            steerInput *
            moveForce.magnitude *
            steerAngle *
            Time.deltaTime
        );
    }

    // ─────────────────────────────

    void HandleDrag()
    {
        float dt = Time.deltaTime;

        float decay = Mathf.Clamp01(drag * dt);

        moveForce = Vector2.Lerp(moveForce, Vector2.zero, decay);
    }

    // ─────────────────────────────

    void HandleDriftPhysics()
    {
        Vector2 forward = transform.up;

        if (moveForce.sqrMagnitude < 0.01f)
        {
            DriftAngle = 0f;
            IsDrifting = false;
            driftGraceTimer = 0f;
            return;
        }

        DriftAngle =
            Vector2.SignedAngle(forward, moveForce.normalized);

        bool driftingNow =
            IsOnRoad() &&
            moveForce.magnitude > 3f &&
            Mathf.Abs(DriftAngle) > 15f;

        if (driftingNow)
            driftGraceTimer = 0.3f;
        else
            driftGraceTimer -= Time.deltaTime;

        IsDrifting = driftGraceTimer > 0f;

        moveForce =
            Vector2.Lerp(
                moveForce,
                forward * moveForce.magnitude,
                traction * Time.deltaTime
            );

        if (leftTrail)
            leftTrail.emitting = IsDrifting;

        if (rightTrail)
            rightTrail.emitting = IsDrifting;
    }

    // ─────────────────────────────

    void Move()
    {
        transform.position += (Vector3)moveForce * Time.deltaTime;
    }

    // ─────────────────────────────

    private void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 normal = col.contacts[0].normal;
        moveForce = Vector2.Reflect(moveForce, normal) * wallBounce;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Road"))
            roadContacts++;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Road"))
            roadContacts--;
    }

    private bool IsOnRoad()
    {
        return roadContacts > 0;
    }

    // ─────────────────────────────

    public void CompleteLap()
    {
        LapCount++;
        OnLapCompleted?.Invoke();
    }

    public void SetCheckpointPassed(bool value)
    {
        checkpointPassed = value;
    }
}