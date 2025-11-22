using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
public class Interactable : MonoBehaviour, IAction
{
    public UnityEvent interactEvent;
    public void Interact()
    {
       interactEvent?.Invoke();
    }

}