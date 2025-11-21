using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class RadioEventSystem : MonoBehaviour
{
    [System.Serializable]
    public class RadioEvent
    {
        public string id;
        public UnityEvent action;
    }

    public List<RadioEvent> events;

    public void TriggerEvent(string id)
    {
        foreach (var e in events)
            if (e.id == id)
                e.action?.Invoke();
    }
}
