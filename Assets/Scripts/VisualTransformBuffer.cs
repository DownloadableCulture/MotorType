using UnityEngine;

/// <summary>
/// Decouples visual rotation from physics by interpolating between fixed timestep frames.
/// This eliminates visual stuttering by providing smooth sub-frame interpolation.
/// </summary>
public class VisualTransformBuffer : MonoBehaviour
{
    [SerializeField] Transform _targetPhysicsTransform;
    [SerializeField] Transform _visualTransform;
    [SerializeField, Range(0f, 1f)] float _interpolationSmoothing = 0.95f;

    private Quaternion _previousPhysicsRotation;
    private Quaternion _currentPhysicsRotation;
    private float _lastPhysicsUpdateTime;
    private float _fixedDeltaTime;

    void Awake()
    {
        if (_targetPhysicsTransform == null)
            _targetPhysicsTransform = GetComponent<Rigidbody>().transform;

        if (_visualTransform == null)
            _visualTransform = transform;

        _fixedDeltaTime = Time.fixedDeltaTime;
        _previousPhysicsRotation = _targetPhysicsTransform.rotation;
        _currentPhysicsRotation = _targetPhysicsTransform.rotation;
        _lastPhysicsUpdateTime = Time.time;
    }

    /// <summary>
    /// Call this at the END of FixedUpdate to capture the physics state.
    /// </summary>
    public void CapturePhysicsState()
    {
        _previousPhysicsRotation = _currentPhysicsRotation;
        _currentPhysicsRotation = _targetPhysicsTransform.rotation;
        _lastPhysicsUpdateTime = Time.time;
    }

    void Update()
    {
        InterpolateVisualRotation();
    }

    private void InterpolateVisualRotation()
    {
        // Calculate time elapsed since last physics frame
        float timeSincePhysicsUpdate = Time.time - _lastPhysicsUpdateTime;

        // Clamp interpolation to [0, 1] range within the physics timestep
        float interpolationFactor = Mathf.Clamp01(timeSincePhysicsUpdate / _fixedDeltaTime);

        // Apply smoothing for even more fluid motion
        interpolationFactor = Mathf.Lerp(interpolationFactor, interpolationFactor, _interpolationSmoothing);

        // Slerp between previous and current physics rotation for smooth visual interpolation
        Quaternion interpolatedRotation = Quaternion.Slerp(
            _previousPhysicsRotation,
            _currentPhysicsRotation,
            interpolationFactor
        );

        _visualTransform.rotation = interpolatedRotation;
    }
}