using UnityEngine;

public class WheelTrailHandler : MonoBehaviour
{
    [SerializeField] private CarController _car;
    private TrailRenderer trailRenderer;

    void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
    }

    void Update()
    {
        if (_car.IsDrifting)
        {
            trailRenderer.emitting = true;
        } else
        {
            trailRenderer.emitting = false;
        }
    }
}
