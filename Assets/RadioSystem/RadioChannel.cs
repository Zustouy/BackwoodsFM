using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Radio/Станция")]
public class RadioChannel : ScriptableObject
{
    [Header("Название станции")]
    public string channelName;

    [Header("Несущая частота Только 136–174 или 400–520 МГц")]
    [Tooltip("Только 136–174 или 400–520 МГц")]
    [CustomRange(136f, 174f, 400f, 520f)]
    public float frequency;

    [Header("Ширина полосы чистого приёма (±МГц)")]
    public float clearRange = 0.15f;

    [Header("Плейлист станции")]
    public List<AudioClip> audioClips;
}