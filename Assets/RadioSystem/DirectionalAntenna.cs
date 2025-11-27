using System.Collections;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DirectionalAntenna : MonoBehaviour
{
    [Header("⎯⎯⎯ Основные параметры антенны ⎯⎯⎯")]
    public AntennaId antennaId;
    public Material material;
    public Transform antennaVisual;
    public float frequency;
    public float frequencyScaning;
    public Vector2Int angleLimit = new(-45, 45);

    [Header("⎯⎯⎯ Настройки точности ⎯⎯⎯")]
    [SerializeField] private float currentAccuracySmooch = 5f;
    [SerializeField] private float maxAcceptableError = 10f;


    [Header("⎯⎯⎯ Отладка ⎯⎯⎯")]
    [SerializeField] private float debugRayLength = 50f;
    [SerializeField] private bool showFlatProjection = true;

    [Header("⎯⎯⎯ Внутренние данные ⎯⎯⎯")]
    [SerializeField] private float angle;
    [SerializeField] private float currentAccuracy;
    [SerializeField] private bool isScaning;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private RescueMissionManager missionManager;


    public void Init(RescueMissionManager manager, AntennaId id)
    {
        GloboalEventManager.OnFrequencyWrite += WriteFrequency;
        missionManager = manager;
        antennaId = id;
        angle = antennaVisual.localEulerAngles.y;
        material.SetFloat("_Accuracy",0);
    }
    private void WriteFrequency(float Write)
    {
        frequency = Write;
    }

    public void RotareAntenna(float lAngle)
    {
        Vector3 e = antennaVisual.localEulerAngles;
        angle = Mathf.Lerp(angleLimit.x, angleLimit.y, lAngle);
        antennaVisual.localEulerAngles = new Vector3(e.x, angle, e.z);
        if (isScaning && frequency == frequencyScaning)
        {
            Vector3 dir = TriangulationUtils.AzimuthToDirection(GetAzimuthDeg());
            float angleError = TriangulationUtils.AngleToTarget(transform.position, dir, targetPosition);
            float progress = TriangulationUtils.GetAccuracyProgress(angleError, maxAcceptableError);
            currentAccuracy = Mathf.Lerp(currentAccuracy, progress, Time.deltaTime *currentAccuracySmooch);
            material.SetFloat("_Accuracy",currentAccuracy);
        }
    }

    public void DoLocalLock()
    {
        float az = GetAzimuthDeg();
        missionManager?.RegisterAntennaLock(antennaId, transform.position, az);
        Debug.Log($"Local lock triggered for {antennaId}, az={az}");
    }

    float GetAzimuthDeg()
    {
        Vector3 f = antennaVisual.forward;
        Vector3 flat = Vector3.ProjectOnPlane(f, Vector3.up).normalized;
        float az = Mathf.Atan2(flat.x, flat.z) * Mathf.Rad2Deg;
        return az;
    }
    public void DoRemoteLock()
    {
        float az = GetAzimuthDeg();
        missionManager?.RegisterAntennaLock(antennaId, transform.position, az);
        Debug.Log($"Remote lock triggered for {antennaId}, az={az}");
    }
    public void StartMission(Vector3 tp, bool missionActive, float frequency)
    {
        frequencyScaning = frequency;
        targetPosition = tp;
        isScaning = missionActive;
    }
    public void EndMission(Vector3 tp, bool missionActive)
    {
        targetPosition = tp;
        isScaning = missionActive;
        StartCoroutine(OffIndicator());
    }
    IEnumerator OffIndicator()
    {
        float index = material.GetFloat("_Accuracy");
        print(index);
        while (index >= 0.1)
        {
            index = Mathf.Lerp(index, 0, Time.deltaTime * 0.5f);
            material.SetFloat("_Accuracy",index);
            yield return null;
        }
        material.SetFloat("_Accuracy",0);
        
    }
    void OnDrawGizmos()
    {
        if (antennaVisual == null) return;

        Vector3 origin = antennaVisual.position;
        Vector3 direction = antennaVisual.forward;
        Vector3 flatDirection = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
        // Дополнительно: проекция на землю (плоскость XZ)
        if (showFlatProjection)
        {
            Debug.DrawRay(origin, flatDirection * debugRayLength * 0.8f, Color.white * 0.7f);
            
            Vector3 endPoint = origin + flatDirection * debugRayLength * 0.8f;
            DrawArrowHead(endPoint, flatDirection, Color.white * 0.8f, 3f);
        }
        // Подпись с текущим азимутом
        float az = GetAzimuthDeg();
        string label = $"{antennaId}\nAz: {az:F1}° {(AntennaId.AntennaB == antennaId ? "(Remote)" : "(Local)")}";
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(origin + Vector3.up * 3f, label);
        #endif
    }
    void DrawArrowHead(Vector3 position, Vector3 direction, Color color, float size = 3f)
    {
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 210, 0) * Vector3.forward;

        Debug.DrawRay(position, right * size, color);
        Debug.DrawRay(position, left * size, color);
    }
}
