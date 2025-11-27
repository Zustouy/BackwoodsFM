using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
public class InteractableRadio : MonoBehaviour, IAction
{
    [Header("⎯⎯⎯ Действие при взаимодействии ⎯⎯⎯")]
    public UnityEvent interactEnterEvent;
    public UnityEvent interactExitEvent;

    bool isSit = false;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)&& isSit)
        {
            CursorManager.HideAndLock();
            interactExitEvent?.Invoke();
            Player.Instance.SetState(PlayerState.Standing);
            isSit = false;
        }

    }
    public void Interact()
    {
        if (!isSit)
        {
            CursorManager.ShowAndUnlock();
            interactEnterEvent?.Invoke();
            Player.Instance.SetState(PlayerState.Sitting);
            isSit = true;
        }
    }
}