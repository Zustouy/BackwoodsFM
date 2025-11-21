using UnityEngine;

[CreateAssetMenu(menuName = "Radio/Anomaly Signal")]
public class RadioSignalAnomaly : ScriptableObject
{
    public string id;
    public float frequency;
    public float clearRange = 0.2f;
    public AudioClip clip;
    public float weirdness = 1.0f; // можно применить эффекты
    public bool triggersEvent = false;
    public string eventName;
}