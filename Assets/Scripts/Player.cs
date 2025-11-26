using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public PlayerState state;
    public Camera mainCamera;
    private float interactableDistance = 1.5f;
    public bool isOnRadio = false;
    public static Player Insyance;
    private HashSet<IHoverable> previous = new HashSet<IHoverable>();
    void Start()
    {
        mainCamera = Camera.main;
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

            // если потеряли hover И нет LockHover
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

