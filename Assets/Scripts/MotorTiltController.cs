using UnityEngine;
using UnityEngine.InputSystem;

public class MotorTiltController : MonoBehaviour
{
    [SerializeField] SteeringWheelController _steeringWheel;
    [SerializeField] float _maxTiltAngle = 12f;
    [SerializeField, Range(0.1f, 10f)] float _tiltSmoothing = 3.5f;

    private Quaternion _targetTiltRotation;
    private float _lastUpdateTime;

    void Start()
    {
        _targetTiltRotation = transform.localRotation;
        _lastUpdateTime = Time.time;
    }

    void Update()
    {
        float targetTilt = _steeringWheel.CurrentSteerValue * _maxTiltAngle;
        _targetTiltRotation = Quaternion.Euler(0f, 0f, targetTilt);

        // Smooth interpolation using frame-independent delta time
        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            _targetTiltRotation,
            Time.deltaTime * _tiltSmoothing
        );
    }
}
