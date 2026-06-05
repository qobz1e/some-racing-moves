using UnityEngine;

public class LapTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        CarController car =
            other.GetComponent<CarController>();

        if (car == null)
            return;

        if (!car.checkpointPassed)
            return;

        car.CompleteLap();
        car.SetCheckpointPassed(false);
    }
}