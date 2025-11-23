using UnityEngine;
public class RadioOnOff : MonoBehaviour
{
    [Header("Radio")]
    public RadioSystem radio;

    [Header("Sounds")]
    public AudioSource clickSource;
    public AudioClip OnSound;    
    public AudioClip OffSound;
    private Camera cam;

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

