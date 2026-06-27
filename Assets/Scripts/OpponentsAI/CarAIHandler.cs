using UnityEngine;
using System.Linq;

public class CarAIHandler : MonoBehaviour
{
    [SerializeField] private CarController _car;
    [SerializeField] private CarController player;

    [SerializeField] private bool isAvoidingCars = true;

    Vector3 targetPosition = Vector3.zero;

    Vector2 avoidanceVectorLerped = Vector3.zero;

    WaypointNode currentWaypoint = null;
    WaypointNode previousWaypoint = null;
    [SerializeField] WaypointNode[] allWaypoints;

    PolygonCollider2D polygonCollider2D;

    private bool playerStarted = false;

    void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    void FixedUpdate()
    {
        if (!playerStarted)
            return;

        Vector2 inputVector = Vector2.zero;

        FollowWaypoints();

        inputVector.x = TurnTowardTarget();
        inputVector.y = ApplyThrottleOrBrake();

        _car.SetInputVector(inputVector);
    }

    void FollowWaypoints()
    {
        if (currentWaypoint == null)
        {
            currentWaypoint = FindClosestWaypoint();
            previousWaypoint = null;
        }

        if (currentWaypoint != null)
        {
            targetPosition = currentWaypoint.transform.position;

            float distanceToWaypoint =
                Vector2.Distance(
                    transform.position,
                    currentWaypoint.transform.position);

            if (distanceToWaypoint > 6)
            {
                Vector3 nearestPointOnLine = FindNearestPointOnLine(
                    previousWaypoint.transform.position,
                    currentWaypoint.transform.position,
                    transform.position);

                float segments = distanceToWaypoint / 6f;

                targetPosition = (targetPosition + nearestPointOnLine * segments) / (segments + 1);
            }

            if (distanceToWaypoint <= currentWaypoint.minDistanceToReachWaypoint)
            {
                previousWaypoint = currentWaypoint;

                currentWaypoint =
                    currentWaypoint.nextWaypointNode[
                        Random.Range(0, currentWaypoint.nextWaypointNode.Length)];
            }
        }
    }

    WaypointNode FindClosestWaypoint()
    {
        return allWaypoints
            .OrderBy(t => Vector3.Distance(transform.position, t.transform.position))
            .FirstOrDefault();
    }

    float TurnTowardTarget()
    {
        Vector2 vectorToTarget =
            (targetPosition - transform.position).normalized;

        if (isAvoidingCars)
            AvoidCars(vectorToTarget, out vectorToTarget);

        float angleToTarget =
            -Vector2.SignedAngle(
                transform.up,
                vectorToTarget);

        return Mathf.Clamp(
            angleToTarget / 45f, -1f, 1f);
    }

    float ApplyThrottleOrBrake()
    {
        return GetCornerThrottle();
    }

    float GetCornerThrottle()
    {
        if (currentWaypoint == null)
            return 1f;

        if (currentWaypoint.nextWaypointNode.Length == 0)
            return 1f;

        WaypointNode next = currentWaypoint.nextWaypointNode[0];

        if (next.nextWaypointNode.Length == 0)
            return 1f;

        WaypointNode nextNext = next.nextWaypointNode[0];

        Vector2 dir1 =
            (next.transform.position -
             currentWaypoint.transform.position).normalized;

        Vector2 dir2 =
            (nextNext.transform.position -
             next.transform.position).normalized;

        float dot = Vector2.Dot(dir1, dir2);

        float cornerFactor =
            Mathf.InverseLerp(-1f, 1f, dot);

        float distance =
            Vector2.Distance(
                transform.position,
                currentWaypoint.transform.position);

        float distanceFactor =
            Mathf.InverseLerp(
                30f,
                0f,
                distance);

        float throttle =
            Mathf.Lerp(
                1f,
                Mathf.Lerp(0.2f, 1f, cornerFactor),
                distanceFactor);

        return throttle;
    }

    Vector2 FindNearestPointOnLine(Vector2 lineStartPosition, Vector2 lineEndPosition, Vector2 point)
    {
        Vector2 lineHeadingVector = (lineEndPosition - lineStartPosition);

        float maxDistance = lineHeadingVector.magnitude;
        lineHeadingVector.Normalize();

        Vector2 lineVectorStartToPoint = point - lineStartPosition;
        float dotProduct = Vector2.Dot(lineVectorStartToPoint, lineHeadingVector);

        dotProduct = Mathf.Clamp(dotProduct, 0f, maxDistance);

        return lineStartPosition + lineHeadingVector * dotProduct;
    }

    bool IsCarInFrontOfAICar(out Vector3 position, out Vector3 otherCarRightVector)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position + transform.up * 0.5f,
            0.3f,
            transform.up,
            6f,
            1 << LayerMask.NameToLayer("Car"));

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
                continue;

            if (hit.collider.gameObject == gameObject)
                continue;

            Debug.DrawRay(
                transform.position,
                transform.up * 3,
                Color.red);

            position = hit.collider.transform.position;
            otherCarRightVector = hit.collider.transform.right;

            return true;
        }

        Debug.DrawRay(
            transform.position,
            transform.up * 3,
            Color.black);

        position = Vector3.zero;
        otherCarRightVector = Vector3.zero;

        return false;
    }

    void AvoidCars(Vector2 vectorToTarget, out Vector2 newVectorToTarget)
    {
        if (IsCarInFrontOfAICar(
            out Vector3 otherCarPosition,
            out Vector3 otherCarRightVector))
        {
            float distanceToCar =
                Vector2.Distance(
                    transform.position,
                    otherCarPosition);

            Vector2 avoidanceVector = Vector2.Reflect(
                (otherCarPosition - transform.position).normalized,
                otherCarRightVector);

            avoidanceVectorLerped =
                Vector2.Lerp(
                    avoidanceVectorLerped,
                    avoidanceVector,
                    Time.fixedDeltaTime * 20f);

            float avoidanceInfluence =
                Mathf.InverseLerp(
                    8f, 
                    1.5f,
                    distanceToCar);

            avoidanceInfluence =
                Mathf.Clamp01(avoidanceInfluence);

            float targetInfluence =
                1f - avoidanceInfluence;

            newVectorToTarget =
                vectorToTarget * targetInfluence +
                avoidanceVectorLerped * avoidanceInfluence * 2f;

            newVectorToTarget.Normalize();

            Debug.DrawRay(
                transform.position,
                avoidanceVectorLerped * 3f,
                Color.green);

            Debug.DrawRay(
                transform.position,
                newVectorToTarget * 3f,
                Color.yellow);

            return;
        }

        avoidanceVectorLerped =
            Vector2.Lerp(
                avoidanceVectorLerped,
                Vector2.zero,
                Time.fixedDeltaTime * 4f);

        newVectorToTarget = vectorToTarget;
    }

    void OnEnable()
    {
        if (player != null)
            player.OnStartedMoving += OnPlayerStartedMoving;
    }

    void OnDisable()
    {
        if (player != null)
            player.OnStartedMoving -= OnPlayerStartedMoving;
    }

    void OnPlayerStartedMoving()
    {
        playerStarted = true;
    }
}
