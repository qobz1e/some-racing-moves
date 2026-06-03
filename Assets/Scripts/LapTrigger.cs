using UnityEngine;

public class LapTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        CarController car =
            other.GetComponent<CarController>();

        if (car != null)
        {
            car.CompleteLap();
        }
    }
}