using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [Header("При входе в триггер")]
    public UnityEvent onTriggerEnterEvent;

    [Header("При выходе из триггера")]
    public UnityEvent onTriggerExitEvent;
    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnterEvent?.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        onTriggerExitEvent?.Invoke();
    }
}
