using UnityEngine;
using UnityEngine.InputSystem;

public class MotorMovement : MonoBehaviour
{
    public InputActionReference Accelerate;

    [SerializeField] float _motorForce = 100f;
    private Rigidbody _rb;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float isPressed = Accelerate.action.ReadValue<float>();

        if (isPressed > 0f)
        {
            _rb.AddForce(transform.forward * _motorForce, ForceMode.Force);
        }
    }
    private void OnEnable()
    {
        Accelerate.action.Enable();
    }

    private void OnDisable()
    {
        Accelerate.action.Disable();
    }
}
