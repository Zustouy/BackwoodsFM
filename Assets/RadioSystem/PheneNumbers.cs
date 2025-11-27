using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Radio/Phone Numb")]
public class PheneNumbers : ScriptableObject
{
[   Header("Номер телефона")]
    public string numb;

    [Header("Аудио ответа")]
    public AudioClip call;

    [Header("Настройки события")]
    public bool triggersEvent = true;
    
    [Header("Действие при звонке")]
    public UnityEvent action;
}
