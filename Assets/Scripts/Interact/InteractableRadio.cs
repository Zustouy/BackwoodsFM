using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
public class InteractableRadio : MonoBehaviour, IAction
{
    public UnityEvent interactEnterEvent;
    public UnityEvent interactExitEvent;
    public Player player;
    bool isSit = false;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)&& isSit)
        {
            interactExitEvent?.Invoke();
            player.SetState(PlayerState.Standing);
            isSit = false;
        }

    }
    public void Interact()
    {
        if (!isSit)
        {
            interactEnterEvent?.Invoke();
            player.SetState(PlayerState.Sitting);
            isSit = true;
        }
    }
}