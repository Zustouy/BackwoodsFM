using UnityEngine;

[CreateAssetMenu(menuName = "Radio/SOS Signal")]
public class RadioSignalSOS : ScriptableObject
{
    public string id;
    public float frequency;
    public float clearRange = 0.1f;
    public AudioClip clip;
    public bool triggersEvent = true;
    public string eventName;
}
