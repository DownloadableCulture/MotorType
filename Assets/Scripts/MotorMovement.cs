using UnityEngine;

public class MotorMovement : MonoBehaviour
{
    [SerializeField] float _motorForce = 100f;
    private Rigidbody _rb;
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
         _rb.AddForce(transform.forward * _motorForce, ForceMode.Force);
    }
}
