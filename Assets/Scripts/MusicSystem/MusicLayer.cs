using UnityEngine;
[System.Serializable]
public class MusicLayer
{
    public LayerType layerType = LayerType.Other;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
}
