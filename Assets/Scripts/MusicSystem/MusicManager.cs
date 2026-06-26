using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Track")]
    [SerializeField] TrackData _trackData;

    [Header("Playback")]
    [SerializeField] bool _playOnStart;
    [SerializeField] bool _loop = true;
    [SerializeField, Range(0f, 1f)] float _masterVolume = 1f;

    readonly List<AudioSource> _audioSources = new List<AudioSource>();
    
    private bool _playOnNextBar;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PreloadAudioSources();
        SubscribeToBPMClock();
        
        if (_playOnStart)
        {
            PlayAssignedTrack();
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PlayOnNextBar();
        }
    }

    void OnDisable()
    {
        UnsubscribeFromBPMClock();
        StopTrack();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void PreloadAudioSources()
    {
        if (_trackData == null || _trackData.layers == null)
            return;

        Debug.Log("[MusicManager] Preloading audio sources");
        
        foreach (MusicLayer layer in _trackData.layers)
        {
            if (layer == null || layer.clip == null)
                continue;

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.clip = layer.clip;
            source.loop = _loop;
            source.volume = 0f; // Silent for preloading
            source.spatialBlend = 0f;
            source.dopplerLevel = 0f;
            source.Play();
            source.Stop();
            
            _audioSources.Add(source);
        }
        
        Debug.Log($"[MusicManager] Preloaded {_audioSources.Count} audio sources");
    }

    private void SubscribeToBPMClock()
    {
        Debug.Log("[MusicManager] Attempting to subscribe to BPMClock");
        
        if (BPMClock.Instance != null)
        {
            Debug.Log("[MusicManager] BPMClock.Instance found, subscribing to OnBarTick");
            BPMClock.Instance.OnBarTick += OnBarTick;
            Debug.Log("[MusicManager] Successfully subscribed to BPMClock.OnBarTick");
        }
        else
        {
            Debug.LogError("[MusicManager] BPMClock.Instance is still NULL in Start()!");
        }
    }

    private void UnsubscribeFromBPMClock()
    {
        if (BPMClock.Instance != null)
        {
            BPMClock.Instance.OnBarTick -= OnBarTick;
            Debug.Log("[MusicManager] Unsubscribed from BPMClock.OnBarTick");
        }
    }

    private void OnBarTick(int barNumber)
    {
        Debug.Log($"[MusicManager] OnBarTick received: Bar {barNumber}");
        
        if (_playOnNextBar)
        {
            Debug.Log($"[MusicManager] Starting playback on bar {barNumber}");
            PlayAssignedTrack();
            _playOnNextBar = false;
        }
    }

    private void PlayOnNextBar()
    {
        Debug.Log("[MusicManager] Space pressed - scheduling playback for next bar");
        _playOnNextBar = true;
    }

    public void PlayAssignedTrack()
    {
        PlayTrack(_trackData);
    }

    public void PlayTrack(TrackData trackData)
    {
        StopTrack();

        if (trackData == null)
        {
            Debug.LogWarning("[MusicManager] No TrackData assigned.");
            return;
        }

        if (trackData.layers == null || trackData.layers.Length == 0)
        {
            Debug.LogWarning($"[MusicManager] Track '{trackData.trackName}' has no layers.");
            return;
        }

        // Reuse preloaded sources
        for (int i = 0; i < trackData.layers.Length; i++)
        {
            MusicLayer layer = trackData.layers[i];
            if (layer == null || layer.clip == null)
            {
                continue;
            }

            AudioSource source = i < _audioSources.Count ? _audioSources[i] : null;
            
            if (source == null)
            {
                Debug.LogWarning($"[MusicManager] Not enough preloaded sources for layer {i}");
                continue;
            }

            source.clip = layer.clip;
            source.loop = _loop;
            source.volume = Mathf.Clamp01(_masterVolume * layer.volume);
            source.Play();

            Debug.Log($"[MusicManager] Playing layer {i}: {layer.clip.name}");
        }
    }

    public void StopTrack()
    {
        foreach (AudioSource source in _audioSources)
        {
            if (source != null && source.isPlaying)
            {
                source.Stop();
            }
        }

        Debug.Log("[MusicManager] All tracks stopped");
    }

    public float GetAssignedTrackBpm()
    {
        if (_trackData == null)
        {
            return 0f;
        }

        return Mathf.Max(0f, _trackData.TrackBPM);
    }

    void OnValidate()
    {
        _masterVolume = Mathf.Clamp01(_masterVolume);
    }
}
