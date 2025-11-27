using UnityEngine;

public class RadioKnob : MonoBehaviour, IDraggable
{
    [Header("⎯⎯⎯ Поворот ручки ⎯⎯⎯")]
    public float sensitivity = 5f;
    public float smooth = 10f;
    public Vector2 angleLimit = new Vector2(-170f, 170f);

    [Header("⎯⎯⎯ Связанный тюнер ⎯⎯⎯")]
    [Tooltip("Скрипт с публичным float angle (например, AntennaTuner)")]
    public Component tunerScript;

    [Header("⎯⎯⎯ Звук трещотки ⎯⎯⎯")]
    public AudioSource clickSource;
    public AudioClip clickSound;
    public float clickAngleStep = 3f;

    [Header("⎯⎯⎯ Состояние перетаскивания ⎯⎯⎯")]
    public bool IsDragging => isDragging;

    [Header("⎯⎯⎯ Внутренние данные ⎯⎯⎯")]
    [SerializeField] private bool isDragging = false;
    [SerializeField] private float targetAngle = 0f;
    [SerializeField] private float lastClickAngle = 0f;
    [SerializeField] private Camera cam;
    [SerializeField, HideInInspector] private System.Reflection.FieldInfo tunerAngleField;

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
        GloboalEventManager.SendDisableCameraMove(true);
        foreach (var h in GetComponents<IHoverable>())
            h.OnHoverEnter();
        foreach (var h in GetComponents<IHoverable>())
            (h as InteractableOutline)?.ForceHover(true);
    }
    private void StopDrag()
    {
        isDragging = false;
        if (Player.Instance.state == PlayerState.Sitting) 
            CursorManager.ShowAndUnlock();
        GloboalEventManager.SendDisableCameraMove(false);
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
