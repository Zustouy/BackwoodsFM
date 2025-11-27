using UnityEngine;
using UnityEngine.Events;
public class Interactable : MonoBehaviour, IAction
{
    [Header("Событие при взаимодействии")]
    public UnityEvent interactEvent;
    public void Interact()
    {
       interactEvent?.Invoke();
    }

}