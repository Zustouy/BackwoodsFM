using UnityEngine;

[RequireComponent(typeof(Outline))]
public class InteractableOutline : MonoBehaviour, IHoverable
{
    public float maxOutlineWidth = 5f;
    public float fadeSpeed = 8f;

    [SerializeField] private MonoBehaviour draggableSource;
    private IDraggable draggable;

    private Outline outline;
    private float targetWidth = 0f;

    private bool forceHover = false;
    public void ForceHover(bool state) => forceHover = state;

    public bool LockHover => forceHover || IsBeingDragged;
    private bool IsBeingDragged => draggable?.IsDragging ?? false;

    void Awake()
    {
        outline = GetComponent<Outline>();
        outline.OutlineWidth = 0f;

        if (draggableSource != null)
        {
            draggable = draggableSource as IDraggable;
            if (draggable == null)
                Debug.LogError($"{draggableSource.name} must implement IDraggable");
        }
    }

    void Update()
    {
        outline.OutlineWidth = Mathf.Lerp(
            outline.OutlineWidth,
            targetWidth,
            Time.deltaTime * fadeSpeed
        );
    }

    public void OnHoverEnter() => targetWidth = maxOutlineWidth;

    public void OnHoverExit()
    {
        if (!LockHover)
            targetWidth = 0f;
    }
    public void ForceExit()
    {
        forceHover = false;          // снимаем защиту
        targetWidth = 0f;            // сразу гасим
    }
}

