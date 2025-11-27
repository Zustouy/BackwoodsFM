using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;


public class Player : MonoBehaviour
{
    [Header("⎯⎯⎯ Синглтон ⎯⎯⎯")]
    public static Player Instance { get; private set; }

    [Header("⎯⎯⎯ Основные ссылки ⎯⎯⎯")]
    public Camera mainCamera;
    public CinemachineInputAxisController cinemachinePanTilt;

    [Header("⎯⎯⎯ Состояние игрока ⎯⎯⎯")]
    public PlayerState state;
    public bool isOnRadio = false;

    [Header("⎯⎯⎯ Настройки взаимодействия ⎯⎯⎯")]
    [SerializeField] private float interactableDistance = 1.5f;

    [Header("⎯⎯⎯ Внутренние данные ⎯⎯⎯")]
    [SerializeField] private HashSet<IHoverable> previous = new HashSet<IHoverable>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        CursorManager.HideAndLock();
        GloboalEventManager.disableCameraMove += disableCameraMove;
        mainCamera = Camera.main;
    }

    private void disableCameraMove(bool isInter)
    {
        if (isInter)
            cinemachinePanTilt.enabled = false;
        else
            cinemachinePanTilt.enabled = true;
    }

    void Update()
    {
        Ray ray = (state == PlayerState.Standing)
            ? new Ray(mainCamera.transform.position, mainCamera.transform.forward)
            : mainCamera.ScreenPointToRay(Input.mousePosition);

        HashSet<IHoverable> current = new HashSet<IHoverable>();

        if (Physics.Raycast(ray, out RaycastHit hit, interactableDistance))
        {
            foreach (var h in hit.collider.GetComponents<IHoverable>())
                if (h != null)
                    current.Add(h);
        }

        // EXIT
        foreach (var old in previous)
        {
            bool stillInside = current.Contains(old);

            if (!stillInside && !old.LockHover)
                old.OnHoverExit();
        }

        // ENTER
        foreach (var now in current)
        {
            if (!previous.Contains(now))
                now.OnHoverEnter();
        }

        previous = current;
    }
    public void SetState(PlayerState newState)
    {
        state = newState;
    }
}

