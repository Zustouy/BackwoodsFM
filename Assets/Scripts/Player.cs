using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public PlayerState state;
    public Camera mainCamera;
    private float IiteractableDistance = 1.5f;
    public bool isOnRadio = false;
    public static Player Insyance;
    protected IHoverable[] previousHoverables;
    void Start()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        if (state == PlayerState.Standing)
        {
            Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward, Color.red);
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, IiteractableDistance))
            {
                var hoverables = hit.collider.GetComponents<IHoverable>();
                //OnHoverEnter
                if (hoverables.Length > 0)
                { 
                    if(hoverables != previousHoverables)
                    {
                        foreach (var hover in hoverables)      
                            hover.OnHoverEnter();
                        previousHoverables = hoverables;
                    }
                }
                // OnHoverExit
                else if (previousHoverables != null)
                {
                    foreach (var previ in previousHoverables)
                        previ.OnHoverExit();
                    previousHoverables = null;
                }
            }
            else
            {
                if (previousHoverables != null)
                {
                    foreach (var prev in previousHoverables)
                        prev.OnHoverExit();
                    previousHoverables = null;
                }
            }
        }
        else
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                var hoverables = hit.collider.GetComponents<IHoverable>();
                //OnHoverEnter
                if (hoverables.Length > 0)
                { 
                    if(hoverables != previousHoverables)
                    {
                        foreach (var hover in hoverables)      
                            hover.OnHoverEnter();
                        previousHoverables = hoverables;
                    }
                }
                // OnHoverExit
                else if (previousHoverables != null)
                {
                    foreach (var previ in previousHoverables)
                        previ.OnHoverExit();
                    previousHoverables = null;
                }
            }
            else
            {
                if (previousHoverables != null)
                {
                    foreach (var prev in previousHoverables)
                        prev.OnHoverExit();
                    previousHoverables = null;
                }
            }
        }
    }
    public void SetState(PlayerState newState)
    {
        state = newState;
    }
}

