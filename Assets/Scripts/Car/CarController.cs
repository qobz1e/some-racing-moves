using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public System.Action OnStartedMoving;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private bool isPlayer;

    private CarData Data;

    [Header("Movement")]
    [SerializeField] private AnimationCurve accelerationCurve =
        AnimationCurve.EaseInOut(0, 0.35f, 1, 0.3f);

    float accelerationInput;
    private Vector2 moveForce;
    public float CurrentSpeed => moveForce.magnitude;

    float Acceleration =>
        Data.stats.acceleration;

    float MaxForwardSpeed =>
        Data.stats.maxSpeed;

    float ReverseAcceleration =>
        Data.stats.reverseAcceleration;

    float ReverseSpeed =>
        Data.stats.reverseSpeed;

    float SpeedMultiplier => isPlayer
        ? 1f + PlayerProfile.Current.EngineLevel * 0.05f
        : 1f;

    float TurboMultiplier => isPlayer
        ? 1f + PlayerProfile.Current.TurboLevel * 0.05f
        : 1f;

    float Accel => Acceleration * TurboMultiplier;

    float Traction => isPlayer
        ? Data.stats.traction *
          (1f + PlayerProfile.Current.TiresLevel * 0.1f)
        : Data.stats.traction + 1f;

    float DragReduce => isPlayer
        ? PlayerProfile.Current.AerodynamicsLevel * 0.02f
        : 0.1f;

    public float MaxSpeed => MaxForwardSpeed * SpeedMultiplier;

    float dragReduce => DragReduce;

    public bool IsReversing { get; private set; }

    [Header("Steering")]
    public float steerAngle = 30f;
    float steerInput;

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

    float NitroCapacity => isPlayer
        ? (PlayerProfile.Current.NitroLevel > 0
            ? 0.5f + PlayerProfile.Current.NitroLevel / 2f
            : 0f)
        : 0f;

    public float NitroAmount { get; private set; }
    private float nitroCooldownTimer;

    private bool nitroInput;

    public bool IsUsingNitro =>
        nitroInput &&
        NitroAmount > 0f;

    public float MaxNitro => NitroCapacity;
    public bool HasNitro => NitroCapacity > 0;
    public float NitroCooldown => nitroCooldownTimer;

    // lap completing
    public int LapCount { get; private set; }
    public bool checkpointPassed { get; private set; }
    public System.Action OnLapCompleted;

    public WaypointNode currentWaypoint;
    public float distanceToNextWaypoint;
    public int waypointIndex { get; private set; }

    public float Progress { get; private set; }

    public bool HasFinished { get; private set; }

    private bool canCountWaypoint = true;

    // slowing down off the road
    private bool isOnRoad;

    private bool movementStarted;

    void Start()
    {
        RaceManager.Instance.RegisterCar(this);

        if (Data == null)
        {
            CarVisual visual = GetComponentInChildren<CarVisual>();

            if (visual != null)
                SetCarData(visual.Data);
        }
    }

    void OnDestroy()
    {
        if (RaceManager.Instance != null)
            RaceManager.Instance.UnregisterCar(this);
    }

    void FixedUpdate()
    {
        if (HasFinished)
        {
            accelerationInput = 0f;
            steerInput = 0f;
        }

        if (IsUsingNitro)
        {
            ActivateNitro();
            NitroAmount -= Time.fixedDeltaTime;

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
                nitroCooldownTimer -= Time.fixedDeltaTime;
            }
            else
            {
                NitroAmount = Mathf.Min(
                    NitroAmount + Time.fixedDeltaTime * 0.5f,
                    NitroCapacity
                );
            }
        }

        HandleAcceleration();

        HandleDrag();

        HandleDriftPhysics();

        UpdateDirectionState();

        HandleSteering();

        if (moveForce.magnitude < 0.1f)
        {
            rb.angularVelocity = 0f;
            rb.linearVelocity = Vector2.zero;
        }

        Move();

        if (!movementStarted && CurrentSpeed > 0.1f)
        {
            movementStarted = true;
            OnStartedMoving?.Invoke();
        }
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
                         Time.fixedDeltaTime;
        }
        else if (accelerationInput < 0f)
        {
            moveForce += (Vector2)transform.up *
                         accelerationInput *
                         ReverseAcceleration *
                         Time.fixedDeltaTime;
        }

        if (!isOnRoad)
        {
            moveForce *= 1f - offRoadDrag * Time.fixedDeltaTime;
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
                            6f * Time.fixedDeltaTime);
        }

        if (speed < -ReverseSpeed)
        {
            Vector2 lateral = moveForce - (Vector2)transform.up * speed;

            moveForce = lateral + (Vector2)transform.up * (-ReverseSpeed);
        }
    }

    void Move()
    {
        rb.MovePosition(rb.position + moveForce * Time.fixedDeltaTime);
    }

    void HandleSteering()
    {
        float forwardDot = Vector2.Dot(moveForce, transform.up);

        float direction = Mathf.Sign(forwardDot);

        if (Mathf.Abs(forwardDot) < 0.05f)
            direction = 1f;

        float reverseMultiplier =
            direction < 0f
                ? 0.6f
                : 1f;

        transform.Rotate(
            Vector3.forward,
            steerInput *
            reverseMultiplier *
            direction *
            moveForce.magnitude *
            steerAngle *
            Time.fixedDeltaTime
        );
    }

    // ─────────────────────────────

    void HandleDrag()
    {
        float currentDrag =
             Mathf.Abs(accelerationInput) > 0.01f
                ? throttleDrag - dragReduce
                : coastDrag;

        float decay = Mathf.Clamp01(currentDrag * Time.fixedDeltaTime);

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
            driftGraceTimer -= Time.fixedDeltaTime;

        IsDrifting = driftGraceTimer > 0f;

        moveForce =
            Vector2.Lerp(
                moveForce,
                forward * moveForce.magnitude,
                Traction * Time.fixedDeltaTime
            );
    }

    // ─────────────────────────────

    private void ActivateNitro()
    {
        moveForce += (Vector2)transform.up * nitroBoostForce * Time.fixedDeltaTime;
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
            isOnRoad = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Road"))
            isOnRoad = false;
    }

    // ─────────────────────────────

    public void CompleteLap()
    {
        if (HasFinished)
            return;

        LapCount++;
        
        OnLapCompleted?.Invoke();
        RaceManager.Instance.OnLapCompleted(this);

        currentWaypoint = null;

        if (LapCount >= RaceManager.Instance.LapsToFinish)
        {
            HasFinished = true;
            RaceManager.Instance.FinishRace(this);
        }
    }

    public void UpdateWaypointProgress(WaypointNode[] allWaypoints)
    {
        if (currentWaypoint == null)
        {
            currentWaypoint = allWaypoints[0];
            waypointIndex = 0;
        }

        distanceToNextWaypoint = 
            Vector3.Distance(
                transform.position,
                currentWaypoint.transform.position
            );

        if (distanceToNextWaypoint <= currentWaypoint.minDistanceToReachWaypoint && canCountWaypoint)
        {
            canCountWaypoint = false;

            if (currentWaypoint.nextWaypointNode != null &&
                currentWaypoint.nextWaypointNode.Length > 0)
            {
                currentWaypoint = currentWaypoint.nextWaypointNode[0];
                waypointIndex++;
            }
        }

        if (isPlayer)
        {
            for (int i = 1; i <= 3; i++)
            {
                int index = waypointIndex + i;

                if (index >= allWaypoints.Length)
                    break;

                if (Vector3.Distance(transform.position,
                    allWaypoints[index].transform.position)
                    < allWaypoints[index].minDistanceToReachWaypoint)
                {
                    waypointIndex = index;
                    currentWaypoint =
                        allWaypoints[index].nextWaypointNode[0];

                    break;
                }
            }
        }

        if (distanceToNextWaypoint > currentWaypoint.minDistanceToReachWaypoint + 1f)
        {
            canCountWaypoint = true;
        }

        Progress =
            LapCount * 10000f +
            waypointIndex * 100f -
            Mathf.Clamp(distanceToNextWaypoint, 0f, 50f);
    }

    public void SetCheckpointPassed(bool value)
    {
        checkpointPassed = value;
        Debug.Log(checkpointPassed);
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

    // ─────────────────────────────

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

    // ─────────────────────────────

    public void SetCarData(CarData data)
    {
        Data = data;
    }
}
