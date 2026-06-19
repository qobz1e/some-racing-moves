using UnityEngine;

public class WaypointNode : MonoBehaviour
{
    [SerializeField] public float minDistanceToReachWaypoint = 5;

    [SerializeField] public WaypointNode[] nextWaypointNode;
}
