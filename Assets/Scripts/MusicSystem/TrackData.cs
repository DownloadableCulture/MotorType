using UnityEngine;

[CreateAssetMenu(fileName = "TrackData", menuName = "Scriptable Objects/TrackData")]
public class TrackData : ScriptableObject
{
    [Header("Metadata")]
    public string trackName;
    public string artistName;

    [Header("AudioData")]
    public float TrackBPM;

    [Header("Stems")]
    public MusicLayer[] layers;

}
