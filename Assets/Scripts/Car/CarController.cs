using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] public float acceleration = 10.5f;
    [SerializeField] public float maxSpeed = 6f;

    [SerializeField] private float reverseAcceleration = 9f;
    [SerializeField] private float reverseSpeed = 3f;

    [SerializeField] private AnimationCurve accelerationCurve =
        AnimationCurve.EaseInOut(0, 0.35f, 1, 0.3f);

    float accelerationInput;
    private Vector2 moveForce;
    float Accel => acceleration * upgradeManager.speedMultiplier;
    float dragReduce => upgradeManager.throttleDecrease;
    public float CurrentSpeed => moveForce.magnitude;
    public float MaxSpeed => maxSpeed * upgradeManager.speedMultiplier;

    [Header("Steering")]
    public float steerAngle = 30f;
    float steerInput;

    [Header("Drift")]
    public float traction = 0.5f;

    public bool IsDrifting { get; private set; }
    public float DriftAngle { get; private set; }
    private float driftGraceTimer;

    [Header("Nitro")]
    [SerializeField] private float nitroBoostForce = 16f;
    [SerializeField] private float nitroRechargeDelay = 5f;
    [SerializeField] private float nitroRechargeDelay2 = 1f;

    [Header("Some Constraints")]
    [SerializeField] private float coastDrag = 2.5f;
    [SerializeField] private float throttleDrag = 0.5f;
    [SerializeField] private float offRoadDrag = 0.8f;
    [SerializeField] private float wallBounce = 0.3f;

    public float NitroAmount { get; private set; }
    private float nitroCooldownTimer;

    private bool IsUsingNitro =>
        Keyboard.current.leftShiftKey.isPressed &&
        NitroAmount > 0f;
    
    public float MaxNitro => upgradeManager.nitroCapacity;
    public bool HasNitro => upgradeManager.nitro.level > 0;
    public float NitroCooldown => nitroCooldownTimer;

    // lap completing
    public int LapCount { get; private set; }
    public bool checkpointPassed { get; private set; }
    public System.Action OnLapCompleted;

    // slowing down off the road
    private int roadContacts = 0;
    private bool isOnRoad;

    void Update()
    {
        if (IsUsingNitro)
        {
            ActivateNitro();
            NitroAmount -= Time.deltaTime;   

            if (NitroAmount <= 0f)
            {
                nitroCooldownTimer = nitroRechargeDelay;
            }
            else
            {
                nitroCooldownTimer = nitroRechargeDelay2;
            }

        }
        else
        {
            if (nitroCooldownTimer > 0f)
            {
                nitroCooldownTimer -= Time.deltaTime;
            }
            else
            {
                NitroAmount = Mathf.Min(
                    NitroAmount + Time.deltaTime * 0.5f,
                    upgradeManager.nitroCapacity
                );
            }
        }

        HandleDrag(); 
        HandleDriftPhysics();
    }

    void FixedUpdate()
    {
        HandleAcceleration();
        HandleSteering();

        if (moveForce.magnitude < 0.1f)
        {
            rb.angularVelocity = 0f;
            rb.linearVelocity = Vector2.zero;
        }

        Move();
    }

    // ─────────────────────────────

    void HandleAcceleration()
    {
        float speedRatio = moveForce.magnitude / MaxSpeed;

        float accel = Accel * accelerationCurve.Evaluate(speedRatio);

        if (accelerationInput > 0f)
        {
            moveForce += (Vector2)transform.up *
                         accelerationInput *
                         accel *
                         Time.deltaTime;
        }
        else if (accelerationInput < 0f)
        {
            moveForce += (Vector2)transform.up *
                         accelerationInput *
                         reverseAcceleration *
                         Time.deltaTime;
        }

        if (!IsOnRoad())
        {
            moveForce *= 1f - offRoadDrag * Time.deltaTime;
        }

        float currentLimit = MaxSpeed;

        if (IsUsingNitro) currentLimit *= 1.5f;

        float speed = Vector2.Dot(moveForce, transform.up);

        if (speed > currentLimit)
        {
            moveForce = moveForce.normalized *
                        Mathf.MoveTowards(
                            speed,
                            currentLimit,
                            6f * Time.deltaTime);
        }

        if (speed < -reverseSpeed)
        {
            Vector2 lateral = moveForce - (Vector2)transform.up * speed;

            moveForce = lateral + (Vector2)transform.up * (-reverseSpeed);
        }
    }

    void Move()
    {
        rb.MovePosition(rb.position + moveForce * Time.deltaTime);
    }

    void HandleSteering()
    {
        float direction =
            Mathf.Sign(
                Vector2.Dot(moveForce, transform.up)
            );

        transform.Rotate(
            Vector3.forward,
            steerInput *
            direction *
            moveForce.magnitude *
            steerAngle *
            Time.deltaTime
        );
    }

    // ─────────────────────────────

    void HandleDrag()
    {
        float currentDrag =
            Keyboard.current.wKey.isPressed
                ? throttleDrag - dragReduce
                : coastDrag;

        float decay = Mathf.Clamp01(currentDrag * Time.deltaTime);

        moveForce = Vector2.Lerp(
            moveForce,
            Vector2.zero,
            decay
        );
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

        DriftAngle = Vector2.SignedAngle(forward, moveForce.normalized);

        bool driftingNow = moveForce.magnitude > 3f && Mathf.Abs(DriftAngle) > 15f;

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

    private void ActivateNitro()
    {
        moveForce += (Vector2)transform.up * nitroBoostForce * Time.deltaTime;
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

    // ─────────────────────────────

    public void SetInputVector(Vector2 inputVector)
    {
        steerInput = -inputVector.x;
        accelerationInput = inputVector.y;
    }
}
