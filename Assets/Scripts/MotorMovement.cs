using UnityEngine;

public class MotorMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MotorInput _motorInputComponent;
    private IMotorInput _motorInput;
    private EngineStateMachine _engineStateMachine;

    [SerializeField] Transform _frontWheel;
    [SerializeField] Transform _rearWheel;
    [SerializeField] Transform _visualBody;

    [Header("Variables")]
    [SerializeField] float _motorForce = 100f;
    [SerializeField] float _steerSpeed = 40f;
    [SerializeField] float _maxSteerAngle = 45f;
    [SerializeField] float _minSteerAngle = 10f;
    [SerializeField, Range(1.5f, 3f)] float _breakModifier = 2f;
    [SerializeField, Range(0f, 1f)] float lateralFriction = 0.85f;
    [SerializeField, Range(0.8f, 1.0f)] float _maxSteeringSpeedReduction = 0.98f;
    [SerializeField, Range(0.95f, 1.0f)] float _minSteeringSpeedReduction = 1.0f;

    public float CurrentSpeed { get; private set; }
    public float PreviousSpeed { get; private set; }

    private float _brakeForce;
    private Rigidbody _rb;
    private readonly float _speedReductionFactor = 12f;
    private float _estimatedMaxSpeed;
    private VisualTransformBuffer _visualTransformBuffer;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _brakeForce = _motorForce * _breakModifier;
        _motorInput = _motorInputComponent;

        var engineSound = GetComponent<EngineSound>();
        _engineStateMachine = new EngineStateMachine(this, engineSound);

        _visualTransformBuffer = GetComponent<VisualTransformBuffer>();
        if (_visualTransformBuffer == null && _visualBody != null)
        {
            _visualTransformBuffer = gameObject.AddComponent<VisualTransformBuffer>();
        }

        // Calculate and print estimated max speed
        _estimatedMaxSpeed = (_rb.linearDamping > 0f ? _motorForce / _rb.linearDamping : float.PositiveInfinity) - _speedReductionFactor;
        Debug.Log($"[MotorMovement] Estimated Max Speed: {_estimatedMaxSpeed:F2} (MotorForce: {_motorForce}, LinearDamping: {_rb.linearDamping})");
    }

    void FixedUpdate()
    {
        PreviousSpeed = CurrentSpeed;

        HandleSteering();
        HandleAcceleration();
        HandleBrake();
        ApplyLateralFriction();
        ApplySteeringSpeedReduction();

        CalculateCurrentSpeed();
        _engineStateMachine.Update();

        // Capture physics state at the end of FixedUpdate for visual interpolation
        _visualTransformBuffer?.CapturePhysicsState();
    }

    void CalculateCurrentSpeed()
    {
        CurrentSpeed = _rb != null ? _rb.linearVelocity.magnitude : 0f;
    }

    void HandleSteering()
    {
        float steerInput = _motorInput != null ? _motorInput.SteerInput : 0f;
        float speed = _rb.linearVelocity.magnitude;
        Vector3 groundNormal = GetApproximateGroundNormal();

        // Constrain steering based on estimated max speed
        float effectiveSteerAngle = Mathf.Lerp(_maxSteerAngle, _minSteerAngle, Mathf.Clamp01(speed / _estimatedMaxSpeed));
        float currentSteerAngle = steerInput * effectiveSteerAngle;

        // 1. Rotate the front wheel based on steering input
        if (_frontWheel != null && _rearWheel != null)
        {
            Vector3 wheelDir = (_frontWheel.position - _rearWheel.position).normalized;
            Quaternion steerRot = Quaternion.AngleAxis(currentSteerAngle, groundNormal);
            _frontWheel.rotation = steerRot * Quaternion.LookRotation(wheelDir, groundNormal);
        }

        // 2. Smoothly rotate the bike body to follow the front wheel's direction ONLY if accelerating
        float accelerationInput = _motorInput != null ? _motorInput.AccelerationInput : 0f;
        if (_frontWheel != null && accelerationInput > 0f)
        {
            Vector3 targetForward = _frontWheel.forward;
            Quaternion targetRotation = Quaternion.LookRotation(
                Vector3.ProjectOnPlane(targetForward, groundNormal),
                groundNormal
            );
            _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, targetRotation, _steerSpeed * Time.fixedDeltaTime));
        }
    }

    void HandleAcceleration()
    {
        float accelerationInput = _motorInput != null ? _motorInput.AccelerationInput : 0f;
        if (accelerationInput <= 0f)
            return;

        Vector3 groundNormal = GetApproximateGroundNormal();
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;
        float engineForce = accelerationInput * _motorForce;

        // Apply force at the rear wheel position for realism
        if (_rearWheel != null)
            _rb.AddForceAtPosition(forward * engineForce, _rearWheel.position, ForceMode.Force);
        else
            _rb.AddForce(forward * engineForce, ForceMode.Force);
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

    void ApplyLateralFriction()
    {
        Vector3 groundNormal = GetApproximateGroundNormal();
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;
        Vector3 right = Vector3.Cross(groundNormal, forward).normalized;

        Vector3 velocity = _rb.linearVelocity;
        float lateralSpeed = Vector3.Dot(velocity, right);
        Vector3 lateralVelocity = right * lateralSpeed;

        _rb.linearVelocity -= lateralVelocity * lateralFriction;
    }

    void ApplySteeringSpeedReduction()
    {
        float steerAmount = Mathf.Abs(_motorInput?.SteerInput ?? 0f);
        if (steerAmount < 0.01f)
            return;

        float reductionFactor = Mathf.Lerp(_minSteeringSpeedReduction, _maxSteeringSpeedReduction, steerAmount);
        _rb.linearVelocity *= reductionFactor;
    }

    private Vector3 GetApproximateGroundNormal()
    {
        if (_frontWheel == null || _rearWheel == null)
            return Vector3.up;

        Vector3 frontPos = _frontWheel.position;
        Vector3 rearPos = _rearWheel.position;
        Vector3 wheelDir = (frontPos - rearPos).normalized;
        Vector3 bikeRight = Vector3.Cross(wheelDir, Vector3.up).normalized;
        Vector3 groundNormal = Vector3.Cross(wheelDir, bikeRight).normalized;
        if (groundNormal.y < 0)
            groundNormal = -groundNormal;
        return groundNormal;
    }
}
