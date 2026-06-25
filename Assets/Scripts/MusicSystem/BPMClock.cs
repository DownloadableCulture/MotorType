using UnityEngine;

/// <summary>
/// A BPM clock for synchronizing audio playback and managing beat-based timing in 4/4 time.
/// Uses AudioSettings.dspTime for precise audio-engine-based timing.
/// </summary>
public class BPMClock : MonoBehaviour
{
    public static BPMClock Instance { get; private set; }

    [Header("BPM Settings")]
    [SerializeField] float _bpm = 120f;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _metronomeClick;

    [Header("Debug")]
    [SerializeField] int _beatInBar;
    [SerializeField][Range(0f, 1f)] float _barProgress;

    private double _startDspTime;
    private float _beatDuration;
    private bool _isRunning;
    private bool _metronomeStarted;
    private float _lastBpm;
    private float _accumulatedBeat;

    // Events
    public delegate void BeatEventHandler(int beatCount);
    public event BeatEventHandler OnBeatTick;
    public event BeatEventHandler OnBarTick;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnValidate() => _bpm = Mathf.Max(1f, _bpm);

    private void OnEnable()
    {
        _isRunning = true;
        _lastBpm = _bpm;
        _startDspTime = AudioSettings.dspTime;
        _accumulatedBeat = 0f;
    }

    private void OnDisable() => _isRunning = false;

    private void Start() => UpdateBeatDuration();

    private void Update()
    {
        if (!_isRunning)
            return;

        // Detect inspector changes to BPM during play
        if (_bpm != _lastBpm)
        {
            SetBPM(_bpm);
            _lastBpm = _bpm;
        }

        double elapsedTime = AudioSettings.dspTime - _startDspTime;
        float currentBeat = _accumulatedBeat + (float)(elapsedTime / _beatDuration);

        // Update beat in bar every frame for smooth visual feedback
        _beatInBar = (Mathf.FloorToInt(currentBeat) % 4) + 1;

        // Update bar progress (0-1 range for the current 4-beat bar)
        _barProgress = currentBeat % 4f / 4f;

        int previousBeat = Mathf.FloorToInt(currentBeat - (float)(Time.deltaTime / _beatDuration));
        int beatFloor = Mathf.FloorToInt(currentBeat);

        if (beatFloor > previousBeat)
        {
            OnBeatTick?.Invoke(beatFloor);

            // Fire bar event every 4 beats
            if (beatFloor % 4 == 0)
            {
                OnBarTick?.Invoke(beatFloor / 4);
                StartMetronome();
            }
        }
    }

    private void StartMetronome()
    {
        if (_metronomeStarted || _metronomeClick == null || _audioSource == null)
            return;

        _audioSource.clip = _metronomeClick;
        _audioSource.loop = true;
        _audioSource.Play();
        _metronomeStarted = true;
    }

    public void SetBPM(float bpm)
    {
        // Store current beat before changing BPM
        double elapsedTime = AudioSettings.dspTime - _startDspTime;
        _accumulatedBeat += (float)(elapsedTime / _beatDuration);

        _bpm = Mathf.Max(1f, bpm);
        UpdateBeatDuration();

        // Reset the timer for the new BPM calculation
        _startDspTime = AudioSettings.dspTime;

        // Adjust metronome pitch to match new BPM
        if (_audioSource != null && _audioSource.isPlaying)
        {
            _audioSource.pitch = _bpm / 120f;
        }
    }

    public float GetBPM() => _bpm;

    public float GetCurrentBeat() => _accumulatedBeat + (float)((AudioSettings.dspTime - _startDspTime) / _beatDuration);

    public void ResetClock()
    {
        _startDspTime = AudioSettings.dspTime;
        _accumulatedBeat = 0f;
        _metronomeStarted = false;
        _beatInBar = 0;
        _barProgress = 0f;
        _audioSource?.Stop();
    }

    private void UpdateBeatDuration() => _beatDuration = 60f / _bpm;
}