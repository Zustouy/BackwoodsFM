using UnityEngine;
public class RadioAntenaLock : MonoBehaviour
{
    [Header("⎯⎯⎯ Звук клика ⎯⎯⎯")]
    public AudioSource clickSource;
    public AudioClip sound;

    [Header("⎯⎯⎯ Связанная антенна ⎯⎯⎯")]
    public DirectionalAntenna antenna;

    [Header("⎯⎯⎯ Тип блокировки ⎯⎯⎯")]
    [SerializeField] private bool isForRemote;

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
                   if (!isForRemote)
                    {
                        clickSource.PlayOneShot(sound);
                        antenna.DoLocalLock();
                    }
                    else
                    {
                        clickSource.PlayOneShot(sound);
                        antenna.DoRemoteLock();
                    }
                }
            }
        }
    }
}

