using UnityEngine;

public class RadioKnob : MonoBehaviour
{
    [Header("Rotation")]
    public float sensitivity = 5f;
    public float smooth = 10f;
    public Vector2 angleLimit = new Vector2(-170f, 170f);
    public Component tunerScript;

    [Header("Sounds")]
    public AudioSource clickSource;
    public AudioClip clickSound;
    public float clickAngleStep = 3f; // через сколько градусов — щелчок

    private bool isDragging = false;
    private float angle = 0f;
    private float lastClickAngle = 0f;
    private Camera cam;


    void Start()
    {
        cam = Camera.main;
        angle = transform.localEulerAngles.y;
    }

    void Update()
    {
        // Наведение и зажатие
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
                if (hit.transform == transform)
                    isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Поворот ручки
        if (isDragging)
        {
            float mouseX = Input.GetAxis("Mouse X");
            angle += mouseX * sensitivity;
            angle = Mathf.Clamp(angle, angleLimit.x, angleLimit.y);
            tunerScript.GetType().GetField("angle").SetValue(tunerScript, angle);
        }
        // Плавная интерполяция
        Vector3 e = transform.localEulerAngles;
        float newAngle = Mathf.LerpAngle(e.z, angle, Time.deltaTime * smooth);
        transform.localEulerAngles = new Vector3(e.x, e.y, newAngle);
        // Щелчки
        HandleClicks(newAngle);


    }
    void HandleClicks(float angle)
    {
        if (Mathf.Abs(angle - lastClickAngle) >= clickAngleStep)
        {
            clickSource.PlayOneShot(clickSound);
            lastClickAngle = angle;
        }
    }
}
