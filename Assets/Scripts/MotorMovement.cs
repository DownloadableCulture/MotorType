using UnityEngine;

public class MotorMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MotorInput _motorInputComponent; // Concrete type for Inspector
    private IMotorInput _motorInput; // Interface for logic

    [SerializeField] Transform _frontWheel;
    [SerializeField] Transform _rearWheel;
    [SerializeField] Transform _visualBody;

    [Header("Variables")]
    [SerializeField] float _motorForce = 100f;
    [SerializeField] float _steerSpeed = 40f;
    [SerializeField] float _maxSteerAngle = 45f;
    [SerializeField, Range(1.5f, 3f)] float _breakModifier = 2f;
    [SerializeField, Range(1f, 20f)] float rotationSmooth = 10f;

    private float _brakeForce;
    private Vector3 _moveDirection;
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _brakeForce = _motorForce * _breakModifier;
        _motorInput = _motorInputComponent; // Assign for interface use
    }

    void HandleSteering()
    {
        float steerInput = _motorInput != null ? _motorInput.SteerInput : 0f;

        _moveDirection =
            Quaternion.Euler(0f, steerInput * _maxSteerAngle, 0f)
            * transform.forward;

        _moveDirection.y = 0f;
        _moveDirection.Normalize();

        if (_rb.linearVelocity.sqrMagnitude > 0.01f)
        {
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
    }

    void HandleAcceleration()
    {
        float accelerationInput = _motorInput != null ? _motorInput.AccelerationInput : 0f;

        if (accelerationInput <= 0f)
            return;

        float engineForce = accelerationInput * _motorForce;
        _rb.AddForce(_moveDirection * engineForce, ForceMode.Force);
    }

    void HandleBrake()
    {
        float brakeInput = _motorInput != null ? _motorInput.BrakeInput : 0f;

        if (brakeInput <= 0f)
            return;

        Vector3 velocity = _rb.linearVelocity;

        if (velocity.sqrMagnitude < 0.01f)
            return;

        Vector3 brakeForce = -velocity.normalized * _brakeForce;
        _rb.AddForce(brakeForce, ForceMode.Force);
    }

    void FixedUpdate()
    {
        HandleSteering();
        HandleAcceleration();
        HandleBrake();
    }

    private void LateUpdate()
    {
        if (_rb.linearVelocity.sqrMagnitude > 0.01f && _visualBody != null)
        {
            _visualBody.rotation = _rb.rotation;
        }
    }
}
