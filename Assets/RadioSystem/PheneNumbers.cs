using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Radio/Phone Numb")]
public class PheneNumbers : ScriptableObject
{
    public string numb;
    public AudioClip call;
    public bool triggersEvent = true;
    public UnityEvent action;
}
