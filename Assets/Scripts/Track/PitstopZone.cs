using UnityEngine;

public class PitStopZone : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        CarController car = other.GetComponent<CarController>();

        if (car == null)
            return;

        if (!car.NeedsPitstop)
            return;

        if (car.CurrentSpeed > 0.5f)
            return;

        car.StartPitstop();
    }
}