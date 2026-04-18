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

        Vector3 groundNormal = GetApproximateGroundNormal();

        // Project forward onto the slope
        Vector3 forwardOnSlope = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;
        Quaternion steerRotation = Quaternion.AngleAxis(steerInput * _maxSteerAngle, groundNormal);
        _moveDirection = steerRotation * forwardOnSlope;

        if (_rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection, groundNormal);

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

        Vector3 groundNormal = GetApproximateGroundNormal();
        Vector3 moveDirectionOnSlope = Vector3.ProjectOnPlane(_moveDirection, groundNormal).normalized;

        float engineForce = accelerationInput * _motorForce;
        _rb.AddForce(moveDirectionOnSlope * engineForce, ForceMode.Force);
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

        // Debug: Print ground normal and tire heights
        if (_frontWheel != null && _rearWheel != null)
        {
            Vector3 groundNormal = GetApproximateGroundNormal();
            float frontHeight = _frontWheel.position.y;
            float rearHeight = _rearWheel.position.y;

            Debug.Log($"Ground Normal: {groundNormal}, Front Tire Height: {frontHeight}, Rear Tire Height: {rearHeight}");
        }
    }

    private void LateUpdate()
    {
        if (_rb.linearVelocity.sqrMagnitude > 0.01f && _visualBody != null)
        {
            _visualBody.rotation = _rb.rotation;
        }
    }

    private Vector3 GetApproximateGroundNormal()
    {
        if (_frontWheel == null || _rearWheel == null)
            return Vector3.up;

        Vector3 frontPos = _frontWheel.position;
        Vector3 rearPos = _rearWheel.position;

        // The direction from rear to front wheel
        Vector3 wheelDir = (frontPos - rearPos).normalized;

        // The right vector of the bike (perpendicular to wheel direction and up)
        Vector3 bikeRight = Vector3.Cross(wheelDir, Vector3.up).normalized;

        // The ground normal is perpendicular to both the wheel direction and the vector from rear to front projected onto the XZ plane
        Vector3 groundNormal = Vector3.Cross(wheelDir, bikeRight).normalized;

        // Ensure the normal points upwards
        if (groundNormal.y < 0)
            groundNormal = -groundNormal;

        return groundNormal;
    }
}
