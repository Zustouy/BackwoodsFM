using UnityEngine;

[RequireComponent(typeof(Outline))]
public class GuidebookInteractable : MonoBehaviour, IAction
{
    [Header("Outline settings")]
    public Outline outline;           
    public float flashSpeed = 2f;     
    public float usedOutlineWidth = 3f;

    [Header("Guide UI")]
    public GameObject guideUI;        

    public bool isFlashing = true;   
    public bool hasBeenUsed = false; 

    private void Start()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        outline.enabled = true;
        outline.OutlineWidth = 0f;
        if (guideUI != null)
            guideUI.SetActive(false);
    }
    public void CloseGuide()
    {

        if (guideUI != null)
            guideUI.SetActive(false);
        CursorManager.HideAndLock();
        hasBeenUsed = false;
        if (!isFlashing)
            outline.OutlineWidth = 0f;
    }

    private void Update()
    {
        if (isFlashing)
        {
            float pulse = (Mathf.Sin(Time.time * flashSpeed) + 1f) / 2f; // 0..1
            outline.OutlineWidth = Mathf.Lerp(0f, usedOutlineWidth + 2f, pulse);
        }
    }
    public void Interact()
    {
        if (guideUI != null)
        {
            CursorManager.ShowAndUnlock();
            guideUI.SetActive(true);
        }

        if (!hasBeenUsed)
        {
            hasBeenUsed = true;
            isFlashing = false;
            outline.OutlineWidth = usedOutlineWidth;
        }
    }
}
