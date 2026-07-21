using UnityEngine;
using UnityEngine.InputSystem;

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
    private int _lastFiredBeat;

    private bool _isBpmTransitionActive;
    private float _transitionStartBpm;
    private float _transitionTargetBpm;
    private float _transitionStartBeat;
    private float _transitionDurationBeats;

    // Events
    public delegate void BeatEventHandler(int beatCount);
    public delegate void BpmTransitionEventHandler(float startBpm, float endBpm);
    public event BeatEventHandler OnBeatTick;
    public event BeatEventHandler OnBarTick;
    public event BpmTransitionEventHandler OnBpmTransitionCompleted;

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
        _lastFiredBeat = -1;
    }

    private void OnDisable() => _isRunning = false;

    private void Start() => UpdateBeatDuration();

    private void Update()
    {
        if (!_isRunning)
            return;

        //HandleDebugInput();
        UpdateBpmState();
        UpdateBeatState();
    }

    private void UpdateBpmState()
    {
        if (_isBpmTransitionActive)
            UpdateBpmTransition();

        if (_bpm != _lastBpm)
            SetBPM(_bpm);
    }

    private void UpdateBeatState()
    {
        float currentBeat = GetCurrentBeat();
        UpdateBeatDisplay(currentBeat);
        DetectBeatEvents(currentBeat);
    }

    private void UpdateBeatDisplay(float currentBeat)
    {
        _beatInBar = (Mathf.FloorToInt(currentBeat) % 4) + 1;
        _barProgress = currentBeat % 4f / 4f;
    }

    private void DetectBeatEvents(float currentBeat)
    {
        int beatFloor = Mathf.FloorToInt(currentBeat);

        if (beatFloor > _lastFiredBeat)
        {
            OnBeatTick?.Invoke(beatFloor);
            _lastFiredBeat = beatFloor;

            if (beatFloor % 4 == 0)
            {
                OnBarTick?.Invoke(beatFloor / 4);
                StartMetronome();
            }
        }
    }

    //private void HandleDebugInput()
    //{
    //    if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
    //    {
    //        ChangeBPMTo125OverTwoBars();
    //    }
    //}

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

        // Keep inspector-change detection in sync
        _lastBpm = _bpm;

        // Adjust metronome pitch to match new BPM
        if (_audioSource != null && _audioSource.isPlaying)
        {
            _audioSource.pitch = _bpm / 120f;
        }
    }

    public void ChangeBPMTo125OverTwoBars()
    {
        StartBPMTransition(125f, 2f);
    }

    public void StartBPMTransition(float targetBpm, float bars)
    {
        _transitionStartBpm = _bpm;
        _transitionTargetBpm = Mathf.Max(1f, targetBpm);
        _transitionStartBeat = GetCurrentBeat();
        _transitionDurationBeats = Mathf.Max(0.01f, bars) * 4f;
        _isBpmTransitionActive = true;
    }

    private void UpdateBpmTransition()
    {
        float beatsElapsed = GetCurrentBeat() - _transitionStartBeat;
        float t = Mathf.Clamp01(beatsElapsed / _transitionDurationBeats);
        float nextBpm = Mathf.Lerp(_transitionStartBpm, _transitionTargetBpm, t);

        SetBPM(nextBpm);

        if (t >= 1f)
        {
            _isBpmTransitionActive = false;
            SetBPM(_transitionTargetBpm);
            OnBpmTransitionCompleted?.Invoke(_transitionStartBpm, _transitionTargetBpm);
            LogBpmTransitionCompleted(_transitionStartBpm, _transitionTargetBpm);
        }
    }

    private void LogBpmTransitionCompleted(float startBpm, float endBpm)
    {
        Debug.Log($"[BPMClock] BPM transition completed: {startBpm} to {endBpm}");
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
        _isBpmTransitionActive = false;
        _lastFiredBeat = -1;
    }

    private void UpdateBeatDuration() => _beatDuration = 60f / _bpm;
}