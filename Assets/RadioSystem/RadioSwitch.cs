using UnityEngine;
using System;

public class RadioSwitch : MonoBehaviour
{
    public float minVHF = 136f;
    public float maxVHF = 174f;
    public float minUHF = 400f;
    public float maxUHF = 520f;
    public bool changeFrequency;

    [Header("Sounds")]
    public AudioSource clickSource;
    public AudioClip clickSound;

    private Camera cam;

    public static event Action<float, float> OnFrequencyRangeChanged;

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
                    changeFrequency = !changeFrequency;
                    clickSource.PlayOneShot(clickSound);
                    if (changeFrequency)
                        OnFrequencyRangeChanged?.Invoke(maxVHF, minVHF);
                    else
                        OnFrequencyRangeChanged?.Invoke(maxUHF, minUHF);
                }
            }
        }
    }
}
