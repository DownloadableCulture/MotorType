using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MusicManager : MonoBehaviour
{
    [Header("Track")]
    [SerializeField] TrackData _trackData;

    [Header("Playback")]
    [SerializeField] bool _playOnStart;
    [SerializeField] bool _loop = true;
    [SerializeField, Range(0f, 1f)] float _masterVolume = 1f;

    readonly List<AudioSource> _activeSources = new List<AudioSource>();

    void Start()
    {
        if (_playOnStart)
        {
            PlayAssignedTrack();
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PlayAssignedTrack();
        }
    }

    void OnDisable()
    {
        StopTrack();
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

        for (int i = 0; i < trackData.layers.Length; i++)
        {
            MusicLayer layer = trackData.layers[i];
            if (layer == null || layer.clip == null)
            {
                continue;
            }

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.clip = layer.clip;
            source.loop = _loop;
            source.volume = Mathf.Clamp01(_masterVolume * layer.volume);
            source.spatialBlend = 0f;
            source.dopplerLevel = 0f;
            source.Play();

            _activeSources.Add(source);
        }

        if (_activeSources.Count == 0)
        {
            Debug.LogWarning($"[MusicManager] Track '{trackData.trackName}' has no valid audio clips.");
        }
    }

    public void StopTrack()
    {
        for (int i = 0; i < _activeSources.Count; i++)
        {
            AudioSource source = _activeSources[i];
            if (source == null)
            {
                continue;
            }

            source.Stop();

            if (Application.isPlaying)
            {
                Destroy(source);
            }
            else
            {
                DestroyImmediate(source);
            }
        }

        _activeSources.Clear();
    }

    void OnValidate()
    {
        _masterVolume = Mathf.Clamp01(_masterVolume);
    }
}
