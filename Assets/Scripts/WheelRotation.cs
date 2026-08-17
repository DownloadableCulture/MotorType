using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    [SerializeField] Transform _wheel;
    [SerializeField] float _rotationSpeed = 360f;
    private MotorMovement _motorMovement;
    
    void Awake()
    {
        if (_wheel == null)
        {
            _wheel = GetComponent<Transform>();
        }

        // Get the MotorMovement component from the parent or this GameObject
        _motorMovement = GetComponentInParent<MotorMovement>();
        if (_motorMovement == null)
        {
            _motorMovement = GetComponent<MotorMovement>();
        }
    }

    void Update()
    {
        if (_motorMovement != null)
        {
            float speed = _motorMovement.CurrentSpeed;
            _wheel.Rotate(Vector3.right * speed * _rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Fallback to constant rotation if MotorMovement is not found
            _wheel.Rotate(Vector3.right * _rotationSpeed * Time.deltaTime);
        }
    }
}
