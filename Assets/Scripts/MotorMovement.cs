using UnityEngine;
using UnityEngine.InputSystem;

public class MotorMovement : MonoBehaviour
{
    public InputActionReference Accelerate;
    public InputActionReference Brake;
    public InputActionReference Steer;

    [SerializeField] float _motorForce = 100f;
    [SerializeField] float _steerSpeed = 40f;
    [SerializeField, Range(1.5f, 3f)] float _breakModifier = 2f;
    [SerializeField] InputSettings _inputSettings;
    private float _brakeForce;
    private Rigidbody _rb;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _brakeForce = _motorForce * _breakModifier;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float accelerationPressed = Accelerate.action.ReadValue<float>();
        float brakePressed = Brake.action.ReadValue<float>(); 
        float steerValue = Steer.action.ReadValue<Vector2>().x;

        Vector3 forward = transform.forward;
        Vector3 velocity = _rb.linearVelocity;

        if (accelerationPressed > 0f)
        {
            _rb.AddForce(transform.forward * _motorForce, ForceMode.Force);
            Debug.Log("Move");
        }

        if (brakePressed > 0f && velocity.sqrMagnitude > 0.01f)
        {
            Vector3 brakeForce = -velocity.normalized * _brakeForce;
            _rb.AddForce(brakeForce, ForceMode.Force);
            Debug.Log("Stopping...");
        }

        if (Mathf.Abs(steerValue) > _inputSettings.DeadZone)
        {
            Quaternion turn = Quaternion.Euler(0f, steerValue * _steerSpeed * Time.fixedDeltaTime, 0f);
            _rb.MoveRotation(_rb.rotation * turn);
        }
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
