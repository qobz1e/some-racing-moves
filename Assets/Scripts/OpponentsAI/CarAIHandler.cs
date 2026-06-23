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
        inputVector.y = ApplyThrottleOrBrake(inputVector.x);

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

            currentWaypoint = currentWaypoint.nextWaypointNode
                [Random.Range(0, currentWaypoint.nextWaypointNode.Length)];
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
            -Vector2.SignedAngle(transform.up, vectorToTarget);

        float steerAmount = angleToTarget / 45.0f;

        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);

        return steerAmount;
    }

    float ApplyThrottleOrBrake(float inputX)
    {
        if (_car.CurrentSpeed > _car.MaxSpeed)
            return 0;

        return 1.05f - Mathf.Abs(inputX) / 1.0f;
    }  
}
