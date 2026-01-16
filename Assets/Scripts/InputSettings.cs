using UnityEngine;

[CreateAssetMenu(menuName = "Input")]
public class InputSettings : ScriptableObject
{
    [Range(0f, 0.1f)]
    public float DeadZone = 0.1f;
}
