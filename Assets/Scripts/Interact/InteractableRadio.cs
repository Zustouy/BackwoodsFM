using Unity.Cinemachine;
using UnityEngine;
public class InteractableRadio : MonoBehaviour, IAction
{
    public CinemachineCamera FPCinemachineCamera;
    public CinemachineCamera RadioCinemachineCamera;
    bool isSit = false;
    private void Start()
    {
        
    }
    public void Interact()
    {
        print("interact");
        if (!isSit)
        {
            print("sit");
            FPCinemachineCamera.Priority.Value = 0;
            RadioCinemachineCamera.Priority.Value = 1;
            isSit = true;
        }
        else
        {
            print("up");
            FPCinemachineCamera.Priority.Value = 1;
            RadioCinemachineCamera.Priority.Value = 0;
            isSit = false;
        }
    }

}