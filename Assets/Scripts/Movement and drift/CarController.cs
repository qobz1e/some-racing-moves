using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private PitstopManager pitstopManager;
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] public float acceleration = 10.5f;
    [SerializeField] public float maxSpeed = 6f;
    [SerializeField] private float reverseAcceleration = 9f;
    [SerializeField] private float reverseSpeed = 3f;
    [SerializeField] private float coastDrag = 2f;
    [SerializeField] private float throttleDrag = 0.5f;
    [SerializeField] private AnimationCurve accelerationCurve =
        AnimationCurve.EaseInOut(0, 0.35f, 1, 0.3f);

    [SerializeField] private float wallBounce = 0.3f;

    private Vector2 moveForce;
    float Accel => acceleration * upgradeManager.speedMultiplier;
    public float CurrentSpeed => moveForce.magnitude;
    public float MaxSpeed => maxSpeed * upgradeManager.speedMultiplier;

    [Header("Steering")]
    public float steerAngle = 30f;

    [Header("Drift")]
    public float traction = 0.5f;
    [SerializeField] private TrailRenderer leftTrail;
    [SerializeField] private TrailRenderer rightTrail;

    public bool IsDrifting { get; private set; }
    public float DriftAngle { get; private set; }
    private float driftGraceTimer;

    // pitstops
    public float TotalDistance { get; private set; }
    public bool IsInPitstop { get; private set; }
    public bool NeedsPitstop => pitstopManager.NeedsPitstop;
    private float pitstopTimer;

    // lap completing
    public int LapCount { get; private set; }
    public bool checkpointPassed { get; private set; }
    public System.Action OnLapCompleted;

    // slowing down off the road
    private int roadContacts = 0;
    private bool isOnRoad;
    float offRoadSpeedLimit = 3f;

    [Header("Nitro")]
    [SerializeField] private float nitroBoostForce = 16f;
    [SerializeField] private float nitroRechargeDelay = 5f;
    [SerializeField] private float nitroRechargeDelay2 = 1f;

    public float NitroAmount { get; private set; }
    private float nitroCooldownTimer;

    private bool IsUsingNitro =>
        Keyboard.current.leftShiftKey.isPressed &&
        NitroAmount > 0f &&
        !pitstopManager.NeedsPitstop;
    
    public float MaxNitro => upgradeManager.nitroCapacity;
    public bool HasNitro => upgradeManager.nitro.level > 0;
    public float NitroCooldown => nitroCooldownTimer;

    //private float brakeFactor;

    void Update()
    {
        //brakeFactor = Keyboard.current.spaceKey.isPressed ? 1f : 0f;

        if (IsInPitstop)
        {
            pitstopTimer -= Time.deltaTime;

            if (pitstopTimer <= 0f)
            {
                IsInPitstop = false;

                pitstopManager.PerformPitstop();
            }

            return;
        }

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

        HandleAcceleration();
        HandleSteering();
        HandleDrag(); 
        HandleDriftPhysics();

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

    void FixedUpdate()
    {
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

        if (!IsOnRoad()) accel *= 0.9f;

        if (pitstopManager.NeedsPitstop) accel *= 0.8f;

        if (Keyboard.current.wKey.isPressed)
        {
            moveForce += (Vector2)transform.up * accel * Time.deltaTime;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            moveForce -= (Vector2)transform.up * reverseAcceleration * Time.deltaTime;
        }

        float currentLimit =
            (IsOnRoad() ? MaxSpeed : offRoadSpeedLimit)
            * pitstopManager.SpeedMultiplier;

        if (IsUsingNitro)
            currentLimit *= 1.5f;

        float speed = moveForce.magnitude;

        if (speed > currentLimit)
        {
            moveForce = moveForce.normalized *
                Mathf.MoveTowards(speed, currentLimit, 6f * Time.deltaTime);
        }

        if (speed < -reverseSpeed)
        {
            Vector2 lateral =
                moveForce - (Vector2)transform.up * speed;

            moveForce =
                lateral + (Vector2)transform.up * (-reverseSpeed);
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
                ? throttleDrag
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

        DriftAngle =
            Vector2.SignedAngle(forward, moveForce.normalized);

        bool driftingNow =
            IsOnRoad() &&
            !IsInPitstop &&
            !pitstopManager.NeedsPitstop &&
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

    void Move()
    {
        Vector3 delta = (Vector3)moveForce * Time.deltaTime;

        rb.MovePosition(rb.position + (Vector2)delta);

        TotalDistance += delta.magnitude * 3;
    }

    private void ActivateNitro()
    {
        moveForce += (Vector2)transform.up * nitroBoostForce * Time.deltaTime;
    }

    // ─────────────────────────────

    public void ResetDistance()
    {
        TotalDistance = 0f;
    }

    public void StartPitstop()
    {
        if (IsInPitstop)
            return;

        IsInPitstop = true;
        pitstopManager.StartService(upgradeManager.pitstopDuration);
        pitstopTimer = upgradeManager.pitstopDuration;

        moveForce = Vector2.zero;
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