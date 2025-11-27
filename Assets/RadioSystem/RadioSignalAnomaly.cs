using UnityEngine;

[CreateAssetMenu(menuName = "Radio/Anomaly Signal")]
public class RadioSignalAnomaly : ScriptableObject
{
    [Header(" ID сигнала ")]
    public string id;

    [Header(" Частота Только 136–174 или 400–520 МГц ")]
    [Tooltip("Только 136–174 или 400–520 МГц")]
    [CustomRange(136f, 174f, 400f, 520f)]
    public float frequency;

    [Header(" Полоса чистого приёма (±) ")]
    public float clearRange = 0.2f;

    [Header(" Аудио аномалии ")]
    public AudioClip clip;

    [Header(" Уровень искажений / странности ")]
    public float weirdness = 1.0f;

    [Header(" Вызов события при приёме ")]
    public bool triggersEvent = false;
    public string eventName;
}