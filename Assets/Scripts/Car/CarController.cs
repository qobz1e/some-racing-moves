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

    public bool IsReversing { get; private set; }

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

    public float NitroAmount { get; private set; }
    private float nitroCooldownTimer;

    private bool nitroInput;

    public bool IsUsingNitro =>
        nitroInput &&
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

    private float impactControlLoss = 0f;
    private float steeringLoss = 0f;
    private float tractionMultiplier = 1f;

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

        UpdateDirectionState();

        HandleSteering();

        if (moveForce.magnitude < 0.1f)
        {
            rb.angularVelocity = 0f;
            rb.linearVelocity = Vector2.zero;
        }

        Move();

        impactControlLoss = Mathf.MoveTowards(
            impactControlLoss,
            0f,
            Time.deltaTime * 0.2f
        );

        steeringLoss = Mathf.MoveTowards(
            steeringLoss,
            0f,
            Time.deltaTime * 0.2f
        );

        tractionMultiplier = Mathf.MoveTowards(
            tractionMultiplier,
            1f,
            Time.deltaTime * 0.2f
        );
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

        if (IsReversing)
            moveForce *= 0.97f;

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
        float forwardDot = Vector2.Dot(moveForce, transform.up);

        float direction = Mathf.Sign(forwardDot);

        if (Mathf.Abs(forwardDot) < 0.05f)
            direction = 1f;

        float steerPower = 1f - steeringLoss;

        float reverseMultiplier =
            direction < 0f
                ? 0.6f
                : 1f;

        transform.Rotate(
            Vector3.forward,
            steerInput *
            steerPower *
            reverseMultiplier *
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
             Mathf.Abs(accelerationInput) > 0.01f
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

        bool driftingNow =
            !IsReversing &&
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
    }

    // ─────────────────────────────

    private void ActivateNitro()
    {
        moveForce += (Vector2)transform.up * nitroBoostForce * Time.deltaTime;
    }

    // ─────────────────────────────

    private void OnCollisionEnter2D(Collision2D col)
    {
        Rigidbody2D otherRb = col.rigidbody;
        if (otherRb == null) return;

        ContactPoint2D contact = col.GetContact(0);

        Vector2 normal = contact.normal;

        Vector2 relativeVelocity =
            rb.linearVelocity - otherRb.linearVelocity;

        float impactStrength =
            Vector2.Dot(relativeVelocity, normal);

        if (impactStrength > 0f)
            return;

        impactStrength = Mathf.Abs(impactStrength);

        impactControlLoss =
            Mathf.Clamp01(
                impactStrength * 0.15f);

        steeringLoss =
            Mathf.Clamp01(
                impactStrength * 0.12f);

        tractionMultiplier = 0.15f;

        impactControlLoss = 
            Mathf.Clamp(
                impactControlLoss + 
                impactStrength * 0.2f,
                0f, 1f);

        float impulseStrength = impactStrength * 1.5f;

        Vector2 impulse = normal * impulseStrength;

        moveForce -= impulse * 0.5f;
        otherRb.AddForce(impulse, ForceMode2D.Impulse);

        // ==============================
        // SPIN EFFECT
        // ==============================

        Vector2 hitPoint = contact.point;
        Vector2 center = rb.worldCenterOfMass;

        Vector2 offset = hitPoint - center;

        float torqueDir = Mathf.Sign(
            offset.x * normal.y - offset.y * normal.x
        );

        float spinStrength =
            impactStrength * 1.2f * offset.magnitude;

        rb.angularVelocity += torqueDir * spinStrength;

        otherRb.angularVelocity -= torqueDir * spinStrength * 0.5f;
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

    public void SetNitroInput(bool value)
    {
        nitroInput = value;
    }

    public Vector2 VelocityDirection =>
        moveForce.sqrMagnitude > 0.01f
            ? moveForce.normalized
            : (Vector2)transform.up;

    void UpdateDirectionState()
    {
        if (moveForce.sqrMagnitude < 0.01f)
        {
            IsReversing = false;
            return;
        }

        float forwardSpeed = Vector2.Dot(moveForce, transform.up);

        IsReversing = forwardSpeed < -0.1f;
    }
}
