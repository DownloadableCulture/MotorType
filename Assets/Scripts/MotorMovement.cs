using UnityEngine;
using UnityEngine.InputSystem;

public class MotorMovement : MonoBehaviour
{
    public InputActionReference Accelerate;
    public InputActionReference Break;

    [SerializeField] float _motorForce = 100f;
    private Rigidbody _rb;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float accelerationPressed = Accelerate.action.ReadValue<float>();
        float breakPressed = Break.action.ReadValue<float>();

        if (accelerationPressed > 0f)
        {
            _rb.AddForce(transform.forward * _motorForce, ForceMode.Force);
        }

    }
    private void OnEnable()
    {
        Accelerate.action.Enable();
        Break.action.Enable();
    }

    private void OnDisable()
    {
        Accelerate.action.Disable();
        Break.action.Disable();
    }
}
