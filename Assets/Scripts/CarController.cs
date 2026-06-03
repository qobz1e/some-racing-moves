using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [SerializeField] private UpgradeManager upgradeManager;

    [Header("Movement")]
    public float acceleration = 15f;
    public float maxSpeed = 6f;
    public float drag = 2f;

    [Header("Steering")]
    public float steerAngle = 30f;

    [Header("Drift")]
    public float traction = 0.1f;

    [SerializeField] private float wallBounce = 0.5f;

    private Vector2 moveForce;

    float Accel => acceleration * upgradeManager.speedMultiplier;

    public float CurrentSpeed => moveForce.magnitude;

    public float MaxSpeed => maxSpeed * upgradeManager.speedMultiplier;

    public bool IsDrifting { get; private set; }

    public float DriftAngle { get; private set; }

    private float driftGraceTimer;

    public int LapCount { get; private set; }

    public System.Action OnLapCompleted;

    void Update()
    {
        HandleAcceleration();
        HandleSteering();
        HandleDrag(); 
        HandleDriftPhysics();
        Move();
    }

    // ─────────────────────────────

    void HandleAcceleration()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            moveForce +=
                (Vector2)transform.up *
                Accel *
                Time.deltaTime;
        }

        moveForce =
            Vector2.ClampMagnitude(moveForce, MaxSpeed);
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
            moveForce.magnitude > 2f &&
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

    // ─────────────────────────────

    public void CompleteLap()
    {
        LapCount++;
        OnLapCompleted?.Invoke();
    }
}