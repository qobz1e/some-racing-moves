using UnityEngine;

public class DriftCamera : MonoBehaviour
{
    [SerializeField] private CarController car;
    [SerializeField] private Transform target;

    [Header("Normal")]
    [SerializeField] private float normalSize = 10f;

    [Header("Drift")]
    [SerializeField] private float driftSize = 6f;

    [SerializeField] private float smooth = 4f;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (target == null || car == null)
            return;

        transform.position =
            new Vector3(
                target.position.x,
                target.position.y,
                -10f
            );

        bool drifting = car.IsDrifting;

        float targetSize =
            drifting ? driftSize : normalSize;

        cam.orthographicSize =
            Mathf.Lerp(
                cam.orthographicSize,
                targetSize,
                smooth * Time.unscaledDeltaTime
            );
    }
}