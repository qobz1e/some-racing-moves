using UnityEngine;
using System.Linq;

public class CarAIHandler : MonoBehaviour
{
    [SerializeField] private CarController _car;
    [SerializeField] private CarController player;

    Vector3 targetPosition = Vector3.zero;

    WaypointNode currentWaypoint = null;
    [SerializeField] WaypointNode[] allWaypoints;

    private bool playerStarted = false;

    void FixedUpdate()
    {
        if (player.CurrentSpeed < 0.1f && !playerStarted)
            return;

        playerStarted = true;

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
        }

        if (currentWaypoint != null)
        {
            targetPosition = currentWaypoint.transform.position;

            float distanceToWaypoint =
                Vector2.Distance(
                    transform.position,
                    currentWaypoint.transform.position);

            if (distanceToWaypoint <= currentWaypoint.minDistanceToReachWaypoint)
            {
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

        float angleToTarget =
            -Vector2.SignedAngle(
                _car.VelocityDirection,
                vectorToTarget);

        float driftBias = 0f;

        if (currentWaypoint != null &&
            currentWaypoint.nextWaypointNode.Length > 0)
        {
            WaypointNode next =
                currentWaypoint.nextWaypointNode[0];

            Vector2 currentDir =
                (currentWaypoint.transform.position -
                 transform.position).normalized;

            Vector2 nextDir =
                (next.transform.position -
                 currentWaypoint.transform.position).normalized;

            float cornerAngle =
                Vector2.SignedAngle(
                    currentDir,
                    nextDir);

            float distance =
                Vector2.Distance(
                    transform.position,
                    currentWaypoint.transform.position);

            float anticipation =
                Mathf.InverseLerp(20f, 0f, distance);

            driftBias =
                cornerAngle *
                -0.2f *
                anticipation;
        }

        float desiredAngle =
            angleToTarget + driftBias;

        float steerAmount =
            Mathf.Clamp(
                desiredAngle / 45f,
                -1f,
                1f);

        return steerAmount;
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
}
