using UnityEngine;

[RequireComponent(typeof(Outline))]
public class InteractableOutline : MonoBehaviour, IHoverable
{
    public float outlineRadius = 5f;
    private Outline outline;
    private void Start()
    {
        outline = GetComponent<Outline>();
        outline.OutlineWidth = 0;
    }
    public  void OnHoverEnter()
    {
        outline.OutlineWidth = outlineRadius;
    }
    public  void OnHoverExit()
    {
        outline.OutlineWidth = 0f;
    }

}