using UnityEngine;

public class RadioKnob : MonoBehaviour, IDraggable
{
    [Header("Rotation")]
    public float sensitivity = 5f;
    public float smooth = 10f;
    public Vector2 angleLimit = new Vector2(-170f, 170f);
    
    [Tooltip("Script with public float angle; or a property.")]
    public Component tunerScript;

    [Header("Sounds")]
    public AudioSource clickSource;
    public AudioClip clickSound;
    public float clickAngleStep = 3f;

    public bool IsDragging => isDragging;
    private bool isDragging = false;

    private float targetAngle = 0f;
    private float lastClickAngle = 0f;
    private Camera cam;

    private System.Reflection.FieldInfo tunerAngleField;

    void Start()
    {
        cam = Camera.main;
        targetAngle = transform.localEulerAngles.z;

        if (tunerScript != null)
        {
            tunerAngleField = tunerScript.GetType().GetField("angle");
            if (tunerAngleField == null)
                Debug.LogError($"{tunerScript.name} does NOT contain a field named 'angle'");
        }
    }

    void Update()
    {
        HandleInput();
        RotateKnob();
        HandleClicks();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                if (hit.transform == transform)
                    StartDrag();
            }
        }

        if (Input.GetMouseButtonUp(0))
            StopDrag();
    }
    private void StartDrag()
    {
        isDragging = true;
        CursorManager.Hide();

        foreach (var h in GetComponents<IHoverable>())
            h.OnHoverEnter();
        foreach (var h in GetComponents<IHoverable>())
            (h as InteractableOutline)?.ForceHover(true);
    }
    private void StopDrag()
    {
        isDragging = false;
        CursorManager.ShowAndUnlock();
        foreach (var h in GetComponents<IHoverable>())
        {
            var outline = h as InteractableOutline;
            if (outline != null)
            {
                outline.ForceHover(false);
                outline.ForceExit();
            }
        }
    }


    private void RotateKnob()
    {
        if (isDragging)
        {
            float delta = Input.GetAxis("Mouse X") * sensitivity;
            targetAngle += delta;
            targetAngle = Mathf.Clamp(targetAngle, angleLimit.x, angleLimit.y);

            tunerAngleField?.SetValue(tunerScript, targetAngle);
        }

        Vector3 e = transform.localEulerAngles;
        float smoothAngle = Mathf.LerpAngle(e.z, targetAngle, Time.deltaTime * smooth);
        transform.localEulerAngles = new Vector3(e.x, e.y, smoothAngle);
    }

    private void HandleClicks()
    {
        if (Mathf.Abs(targetAngle - lastClickAngle) >= clickAngleStep)
        {
            clickSource?.PlayOneShot(clickSound);
            lastClickAngle = targetAngle;
        }
    }
}
