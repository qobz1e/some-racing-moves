using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        CarController car =
            other.transform.root.GetComponent<CarController>();

        if (car != null)
        {
            car.SetCheckpointPassed(true);
        }
    }
}