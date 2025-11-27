using ActionCode.Attributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Radio/SOS Сигнал")]
public class RadioSignalSOS : ScriptableObject
{
    [Header("Идентификатор сигнала")]
    public string id;

    [Header("Частота бедствия Только 136–174 или 400–520 МГц")]
    [Tooltip("Только 136–174 или 400–520 МГц")]
    [CustomRange(136f, 174f, 400f, 520f)]
    public float frequency;

    [Header("Диапазон чистого приёма (±МГц)")]
    public float clearRange = 0.1f;

    [Header("Аудио SOS-сообщения")]
    public AudioClip clip;
    
    [Header("Зацикленное  ли SOS-сообщение зацткленым")]
    public bool  isLoop;

    [Header("Время на спасение (сек)")]
    public float timer;

    [Header("Триггер события при обнаружении")]
    public bool triggersEvent = true;

    [Header("Имя вызываемого события")]
    [ShowIf(nameof(triggersEvent))] 
    public string eventName;
}