using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class SteeringWheelController : MonoBehaviour
{
    public InputActionReference Steer;
    [SerializeField] float _maxRotationAngle = 30f;
    [SerializeField] float _returnSpeed = 12f;
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

        if (Mathf.Abs(steerValue) < _inputSettings.DeadZone)
            steerValue = 0f;
        

            float targetRotationZ = steerValue * _maxRotationAngle;

            float currentRotationZ = transform.localEulerAngles.z;
            if (currentRotationZ > 180f)
                currentRotationZ -= 360f;

            float newRotationZ = Mathf.Lerp(
                currentRotationZ,
                targetRotationZ,
                Time.deltaTime * _returnSpeed
            );

            transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            newRotationZ
         );
        
    }
}
