using UnityEngine;
public class RadioOnOff : MonoBehaviour
{
    [Header("⎯⎯⎯ Система радио ⎯⎯⎯")]
    public RadioSystem radio;

    [Header("⎯⎯⎯ Звуки включения/выключения ⎯⎯⎯")]
    public AudioSource clickSource;
    public AudioClip OnSound;
    public AudioClip OffSound;

    [Header("⎯⎯⎯ Внутренние данные ⎯⎯⎯")]
    [SerializeField] private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                   if (!radio.isOn)
                    {
                        clickSource.PlayOneShot(OnSound);
                        radio.On();
                    }
                    else
                    {
                        clickSource.PlayOneShot(OffSound);
                        radio.Off();
                    }
                }
            }
        }
    }
}

