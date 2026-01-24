using UnityEngine;
using UnityEngine.InputSystem;

public class MotorTiltController : MonoBehaviour
{
    [SerializeField] SteeringWheelController _steeringWheel;
    [SerializeField] float _maxTiltAngle = 12f;
    [SerializeField] float _tiltSpeed = 3.5f;

    void Start()
    {
        
    }


    void Update()
    {
        float targetTilt = _steeringWheel.CurrentSteerValue * _maxTiltAngle;

        transform.localRotation = Quaternion.Lerp(
        transform.localRotation,
        Quaternion.Euler(0f, 0f, targetTilt),
        Time.deltaTime * _tiltSpeed
        );
    }
}
