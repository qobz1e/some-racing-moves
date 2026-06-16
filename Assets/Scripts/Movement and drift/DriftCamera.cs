using UnityEngine;

public class DriftCamera : MonoBehaviour
{
    [SerializeField] private CarController car;
    [SerializeField] private Transform target;

    [Header("Camera")]
    [SerializeField] private float fixedSize = 8f;
    [SerializeField] private float smooth = 4f;

    [Header("Camera Bounds")]
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (target == null || car == null)
            return;

        Vector3 desiredPosition = new Vector3(
            target.position.x,
            target.position.y,
            -10f
        );

        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        // clamp position inside bounds
        desiredPosition.x = Mathf.Clamp(
            desiredPosition.x,
            minBounds.x + horzExtent,
            maxBounds.x - horzExtent
        );

        desiredPosition.y = Mathf.Clamp(
            desiredPosition.y,
            minBounds.y + vertExtent,
            maxBounds.y - vertExtent
        );

        transform.position = desiredPosition;

        cam.orthographicSize =
            Mathf.Lerp(
                cam.orthographicSize,
                fixedSize,
                smooth * Time.unscaledDeltaTime
            );
    }
}