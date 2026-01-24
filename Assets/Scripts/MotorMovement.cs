using UnityEngine;
using UnityEngine.InputSystem;

public class MotorMovement : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference Accelerate;
    public InputActionReference Brake;
    public InputActionReference Steer;

    [Header("References")]
    [SerializeField] SteeringWheelController _steeringWheel;
    [SerializeField] Transform _frontWheel;
    [SerializeField] Transform _rearWheel;
    [SerializeField] InputSettings _inputSettings;

    [Header("Variables")]
    [SerializeField] float _motorForce = 100f;
    [SerializeField] float _steerSpeed = 40f;
    [SerializeField] float _maxSteerAngle = 45f;
    [SerializeField, Range(1.5f, 3f)] float _breakModifier = 2f;
    
    private float _brakeForce;
    private float _accelerationInput;
    private float _brakeInput;
    private float _steerInput;
    private Vector3 _moveDirection;

    private Rigidbody _rb;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _brakeForce = _motorForce * _breakModifier;
    }
    void ReadInput()
    {
        _accelerationInput = Accelerate.action.ReadValue<float>();
        _brakeInput = Brake.action.ReadValue<float>();
        _steerInput = -_steeringWheel.CurrentSteerValue;
    }

    void HandleSteering()
    {
        _moveDirection =
    Quaternion.Euler(0f, _steerInput * _maxSteerAngle, 0f)
    * transform.forward;

        _moveDirection.y = 0f;
        _moveDirection.Normalize();

        Quaternion targetRotation =
            Quaternion.LookRotation(_moveDirection, Vector3.up);

        _rb.MoveRotation(
            Quaternion.RotateTowards(
                _rb.rotation,
                targetRotation,
                _steerSpeed * Time.fixedDeltaTime
            )
        );
    }
    void HandleAcceleration()
    {
        if (_accelerationInput <= 0f)
            return;

        _rb.AddForce(_moveDirection * _motorForce, ForceMode.Force);
    }
    void HandleBrake()
    {
        if (_brakeInput <= 0f)
            return;

        Vector3 velocity = _rb.linearVelocity;

        if (velocity.sqrMagnitude < 0.01f)
            return;

        Vector3 brakeForce = -velocity.normalized * _brakeForce;
        _rb.AddForce(brakeForce, ForceMode.Force);
    }
    void FixedUpdate()
    {
        ReadInput();
        HandleSteering();
        HandleAcceleration();
        HandleBrake();
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
