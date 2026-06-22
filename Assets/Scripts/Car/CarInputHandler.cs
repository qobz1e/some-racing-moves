using UnityEngine;
using UnityEngine.InputSystem;

public class CarInputHandler : MonoBehaviour
{
    [SerializeField] private CarController _car;

    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        bool nitro =
            Input.GetKey(KeyCode.LeftShift);

        _car.SetInputVector(inputVector);
        _car.SetNitroInput(nitro);
    }
}