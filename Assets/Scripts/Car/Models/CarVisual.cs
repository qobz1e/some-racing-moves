using UnityEngine;

public class CarVisual : MonoBehaviour
{
    [SerializeField] private CarData carData;

    public CarData Data => carData;

    public PolygonCollider2D PolygonCollider =>
        GetComponent<PolygonCollider2D>();
}