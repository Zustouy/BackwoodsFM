using System.Threading;
using ActionCode.Attributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Radio/SOS Signal")]
public class RadioSignalSOS : ScriptableObject
{
    public string id;
    public float frequency;
    public float clearRange = 0.1f;
    public AudioClip clip;
    public float timer;
    public bool triggersEvent = true;
    [ShowIf(nameof(triggersEvent))]
    public string eventName;
}
