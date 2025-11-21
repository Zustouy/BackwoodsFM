using UnityEngine;

[CreateAssetMenu(menuName = "Radio/Channel")]
public class RadioChannel : ScriptableObject
{
    public string channelName;
    public float frequency;
    public float clearRange = 0.15f;
    public AudioClip audioClip;
}
