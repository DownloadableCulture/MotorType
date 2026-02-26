using UnityEngine;
using UnityEngine.InputSystem;

public class MotorInput : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference Accelerate;
    public InputActionReference Brake;

    [Header("References")]
    [SerializeField] SteeringWheelController _steeringWheel;

    public float AccelerationInput { get; private set; }
    public float BrakeInput { get; private set; }
    public float SteerInput { get; private set; }

    void Update()
    {
        AccelerationInput = Accelerate.action.ReadValue<float>();
        BrakeInput = Brake.action.ReadValue<float>();
        SteerInput = -_steeringWheel.CurrentSteerValue;
    }

    private void OnEnable()
    {
        Accelerate.action.Enable();
        Brake.action.Enable();
    }

    private void OnDisable()
    {
        Accelerate.action.Disable();
        Brake.action.Disable();
    }
}
