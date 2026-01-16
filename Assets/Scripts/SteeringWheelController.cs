using UnityEngine;
using UnityEngine.InputSystem;

public class SteeringWheelController : MonoBehaviour
{
    public InputActionReference Steer;
    [SerializeField] float _maxRotationAngle = 30f;
    [SerializeField] InputSettings _inputSettings;

    private void OnEnable()
    {
        Steer.action.Enable();
    }

    private void OnDisable()
    {
        Steer.action.Disable();
    }
    
    private void Update()
    {
        Vector2 leftStick = Steer.action.ReadValue<Vector2>();

        float steerValue = -leftStick.x;

        if (Mathf.Abs(steerValue) > _inputSettings.DeadZone)
        {
            float rotationZ = steerValue * _maxRotationAngle;
            transform.localEulerAngles = new Vector3(
                transform.localEulerAngles.x,
                transform.localEulerAngles.y,
                rotationZ
            );
        }
    }
}
