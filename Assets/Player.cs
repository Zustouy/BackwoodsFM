using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera mainCamera;
    private float IiteractableDistance = 1.5f;
    public static Player Insyance;
    protected IHoverable[] previousHoverables;
    void Start()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward, Color.red);
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, IiteractableDistance))
        {
            var hoverables = hit.collider.GetComponents<IHoverable>();
            var actions = hit.collider.GetComponents<IAction>();

            if (Input.GetMouseButtonDown(0))
                foreach (var action in actions)
                    action.Interact();
            //OnHoverEnter
            if (hoverables != null)
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
