using UnityEngine;

public class KnobRotate : MonoBehaviour
{
    enum xyz{}
    public float sensitivity = 5f;
    public float smooth = 10f;
    public Vector2 minMax = new Vector2(-170, 170);

    private bool isDragging = false;
    private float targetRotation = 0f;
    private Camera cam;
    public Component tunerScript;

    void Start()
    {
        cam = Camera.main;
        targetRotation = transform.localEulerAngles.z;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                if (hit.transform == transform)
                    isDragging = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
            isDragging = false;
            
        if (isDragging)
        {
            float mouseX = Input.GetAxis("Mouse X");
            targetRotation += mouseX * sensitivity;
            targetRotation = Mathf.Clamp(targetRotation, minMax.x, minMax.y);
        }

        Vector3 current = transform.localEulerAngles;
        float z = Mathf.LerpAngle(current.z, targetRotation, Time.deltaTime * smooth);
        transform.localEulerAngles = new Vector3(current.x, current.y, z);
        tunerScript.GetType().GetField("angle").SetValue(tunerScript, targetRotation);
    }
}
