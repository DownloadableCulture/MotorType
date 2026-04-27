using UnityEngine;

public class EngineSound : MonoBehaviour
{
    [SerializeField] AnimationCurve _pitchCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] float _minPitch = 0.8f;
    [SerializeField] float _maxPitch = 1.3f;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _engineIdle;
    [SerializeField] AudioClip _engineAccelerate;
    [SerializeField] AudioClip _engineFullSpeed;

    public void PlayIdle()
    {
        PlayClip(_engineIdle);
        Debug.Log("[EngineSound] Playing Idle Sound");

    }

    public void PlayAccelerate()
    {
                PlayClip(_engineAccelerate);
    }

    public void PlayFullSpeed()
    {
        PlayClip(_engineFullSpeed);
    }

    private void PlayClip(AudioClip clip)
    {
        if (_audioSource.clip == clip && _audioSource.isPlaying)
            return;
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void UpdatePitch(float normalizedValue)
    {
        float t = Mathf.Clamp01(normalizedValue);
        float curveValue = _pitchCurve.Evaluate(t);
        float pitch = Mathf.Lerp(_minPitch, _maxPitch, curveValue);
        _audioSource.pitch = pitch;
        Debug.Log($"[EngineSound] Updated Pitch: {pitch} (Normalized Value: {normalizedValue})");
    }


}
