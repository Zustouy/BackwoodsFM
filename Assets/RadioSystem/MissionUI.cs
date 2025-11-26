using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    [Header("UI References")]
    public Text timerText;
    public Text statusText;
    public Text hintText;

    [Header("Target marker")]
    public Transform targetMarkerPrefab; // префаб маркера (необязательно)
    public float markerVerticalOffset = 0.2f;
    public float markerLerpSpeed = 8f;

    private Transform currentMarker;
    private Vector3 markerTargetPos;
    private Coroutine hideStatusCoroutine;

    void Start()
    {
        ClearAll();
    }

    #region Public API (вызовы из менеджера миссий)

    public void ShowMissionStarted(float durationSeconds)
    {
        SetStatus($"Mission started — time: {durationSeconds:F0}s");
        SetTimer(durationSeconds);
        hintText.text = "Введите частоту и зафиксируйте антенну A.";
        DestroyMarker();
    }

    public void UpdateTimeRemaining(float seconds)
    {
        SetTimer(seconds);
    }

    public void ShowTriangulationFailed()
    {
        SetStatus("Triangulation failed. Take another reading or request remote operator to re-lock.");
        hintText.text = "Угол слишком параллелен — сделайте повторный замер.";
    }

    // Показывает сообщение о найденной позиции и отображает маркер
    // confidence 0..1
    public void ShowTargetLocked(Vector3 worldPosition, float confidence)
    {
        SetStatus($"Target locked (confidence {confidence * 100f:F0}%)");
        hintText.text = "Подойдите к области или вызовите помощь с телефона.";

        // ставим/перемещаем маркер
        if (targetMarkerPrefab != null)
        {
            if (currentMarker == null)
            {
                currentMarker = Instantiate(targetMarkerPrefab, worldPosition + Vector3.up * markerVerticalOffset, Quaternion.identity);
            }
            markerTargetPos = worldPosition + Vector3.up * markerVerticalOffset;
        }
    }

    public void ShowMissionFailed()
    {
        SetStatus("Mission failed! Time expired.");
        hintText.text = "Жертва не выжила.";
        // можно проиграть звук/анимацию здесь (не реализовано)
    }

    public void ShowMissionCompleted()
    {
        SetStatus("Mission completed! Help is on the way.");
        hintText.text = "";
        // можно воспроизвести звук/эффект успеха
    }

    public void ShowNotCloseEnough(float requiredDistance)
    {
        SetStatus($"Too far from estimated location. Get within {requiredDistance} m.");
        hintText.text = "Подойдите ближе к найденной позиции перед звонком.";
    }

    // Общие сообщения для статуса
    public void ShowStatus(string message)
    {
        SetStatus(message);
    }

    #endregion

    #region Helpers

    private void SetTimer(float seconds)
    {
        if (timerText == null) return;
        if (seconds < 0) seconds = 0;
        int s = Mathf.CeilToInt(seconds);
        int mins = s / 60;
        int secs = s % 60;
        timerText.text = $"{mins:00}:{secs:00}";
    }

    private void SetStatus(string message, float autoHideAfter = 0f)
    {
        if (statusText == null) return;
        statusText.text = message;

        if (hideStatusCoroutine != null) StopCoroutine(hideStatusCoroutine);
        if (autoHideAfter > 0f)
        {
            hideStatusCoroutine = StartCoroutine(HideStatusAfter(autoHideAfter));
        }
    }

    private IEnumerator HideStatusAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        if (statusText != null) statusText.text = "";
        hideStatusCoroutine = null;
    }

    private void ClearAll()
    {
        if (timerText != null) timerText.text = "";
        if (statusText != null) statusText.text = "";
        if (hintText != null) hintText.text = "";
        DestroyMarker();
    }

    private void DestroyMarker()
    {
        if (currentMarker != null)
        {
            Destroy(currentMarker.gameObject);
            currentMarker = null;
        }
    }

    void Update()
    {
        // плавно двигаем маркер к целевой позиции (если есть)
        if (currentMarker != null)
        {
            currentMarker.position = Vector3.Lerp(currentMarker.position, markerTargetPos, Time.deltaTime * markerLerpSpeed);
        }
    }

    #endregion
}
