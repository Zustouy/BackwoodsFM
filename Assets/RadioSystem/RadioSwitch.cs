using UnityEngine;

public class RadioSwitch : MonoBehaviour
{
    [Header("Диапазон VHF (авиация)")]
    public float minVHF = 136f;
    public float maxVHF = 174f;

    [Header("Диапазон UHF (военный/спасательный)")]
    public float minUHF = 400f;
    public float maxUHF = 520f;

    [Header("Звук переключения")]
    public AudioSource clickSource;
    public AudioClip clickSound;

    [Header("Текущее состояние")]
    [SerializeField] private bool changeFrequency = false;

    [Header("Внутренние данные")]
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
                    changeFrequency = !changeFrequency;
                    clickSource.PlayOneShot(clickSound);
                    if (!changeFrequency)
                        GloboalEventManager.SensOnFrequencyRangeChanged(maxVHF, minVHF);
                    else
                        GloboalEventManager.SensOnFrequencyRangeChanged(maxUHF, minUHF);
                }
            }
        }
    }
}
