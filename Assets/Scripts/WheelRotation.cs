using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    [SerializeField] Transform _wheel;
    [SerializeField] float _rotationSpeed = 360f;
    
    void Awake()
    {
        if (_wheel == null)
        {
           _wheel = GetComponent<Transform>();
        }
    }


    void Update()
    {
        _wheel.Rotate(Vector3.right * _rotationSpeed * Time.deltaTime);
    }
}
