using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RescueMissionManager : MonoBehaviour
{
    [Header("Antennas")]
    public DirectionalAntenna antennaA; // локальная (A)
    public DirectionalAntenna antennaB; // удалённая (B)

    [Header("Mission area")]
    public Vector3 missionAreaCenter = Vector3.zero;
    public float missionAreaRadius = 200f;

    [Header("References")]
    public GameObject prefabScream;
    public Triangulator triangulator;
    public PhoneInteraction phoneInteraction;
    public MissionUI missionUI;

    [Header("Tuning")]
    public float requiredLockAngleDiffDeg = 0.1f;

    public UnityEvent OnMissionStarted;
    public UnityEvent OnMissionFailed;
    public UnityEvent OnMissionCompleted;

    [Header("Debug / Visualization")]
    public bool debugShowTarget = true;
    public GameObject targetDebugMarkerPrefab;

    private bool missionActive = false;
    private Vector3 targetPosition;
    private float missionEndTime;
    private AntennaMeasurement measA;
    private AntennaMeasurement measB;
    private bool hasA = false;
    private bool hasB = false;
    private bool triangulated = false;
    private Vector3 lastEstimated;

    void Start()
    {
        if (antennaA != null) antennaA.Init(this, AntennaId.AntennaA);
        if (antennaB != null) antennaB.Init(this, AntennaId.AntennaB);
        GloboalEventManager.OnSignalDetected += StartMission;
        if (phoneInteraction != null) phoneInteraction.OnPhoneCalled += HandlePhoneCalled;
    }

    public void StartMission(float frequency, float missionDurationSeconds)
    {
        if (missionActive) return;

        missionActive = true;
        targetPosition = GenerateRandomTarget();
        antennaA.StartMission(targetPosition, missionActive);
        antennaB.StartMission(targetPosition, missionActive);
        missionEndTime = Time.time + missionDurationSeconds;
        hasA = hasB = triangulated = false;
        lastEstimated = Vector3.zero;
        missionUI?.ShowMissionStarted(missionDurationSeconds);
        if (triangulator != null) triangulator.SetTrueTarget(targetPosition);
        OnMissionStarted?.Invoke();
        StartCoroutine(MissionTick());
        Debug.Log($"Mission started (freq {frequency}). True target: {targetPosition}");
        
    }
    IEnumerator MissionTick()
    {
        while (missionActive)
        {
            if (Time.time >= missionEndTime)
            {
                missionActive = false;
                Instantiate(prefabScream, targetPosition, Quaternion.identity);
                GloboalEventManager.SendOnMissionTimeout();
                antennaA.EndMission(targetPosition, missionActive);
                antennaB.EndMission(targetPosition, missionActive);
                missionUI?.ShowMissionFailed();
                OnMissionFailed?.Invoke();
                Debug.Log("Mission failed: timeout");
                yield break;
            }
            missionUI?.UpdateTimeRemaining(missionEndTime - Time.time);
            yield return null;
        }
    }
    private Vector3 GenerateRandomTarget()
    {
        Vector2 r = Random.insideUnitCircle * missionAreaRadius;
        Vector3 pos = missionAreaCenter + new Vector3(r.x, 0f, r.y);
        pos.y = missionAreaCenter.y;
        return pos;
    }
    public void RegisterAntennaLock(AntennaId id, Vector3 antennaPos, float azimuthDeg)
    {
        var measurement = new AntennaMeasurement
        {
            pos = antennaPos,
            az = azimuthDeg,
        };

        if (id == AntennaId.AntennaA)
        {
            measA = measurement;
            if (!hasA)
            {
                hasA = true;
                missionUI?.ShowStatus("Antenna A locked. Waiting for Antenna B...");
                Debug.Log($"Antenna A locked: az = {azimuthDeg:F1}°");
            }
            else
            {
                Debug.Log($"Antenna A updated: az = {azimuthDeg:F1}° → {azimuthDeg:F1}°");
            }
        }
        else if (id == AntennaId.AntennaB)
        {
            measB = measurement;
            if (!hasB)
            {
                hasB = true;
                missionUI?.ShowStatus("Antenna B locked. Triangulating...");
                Debug.Log($"Antenna B locked: az = {azimuthDeg:F1}°");
            }
            else
            {
                Debug.Log($"Antenna B updated: az = {azimuthDeg:F1}° → {azimuthDeg:F1}°");
            }
        }
        TryTriangulate();
    }
    private void TryTriangulate()
    {
        if (!hasA || !hasB) return;

        float angleDiff = Mathf.Abs(Vector3.Angle(AzimuthToDirection(measA.az), AzimuthToDirection(measB.az)));
        if (angleDiff < requiredLockAngleDiffDeg)
        {
            missionUI?.ShowStatus($"Angles too close ({angleDiff:F1}°). Need at least {requiredLockAngleDiffDeg}°.");
            triangulated = false;
            return;
        }

        bool success = triangulator.TryTriangulate(
            measA.pos, measA.az,
            measB.pos, measB.az,
            out Vector3 estimated, out float confidence);

        if (!success)
        {
            missionUI?.ShowTriangulationFailed();
            triangulated = false;
            return;
        }
        Debug.Log($"Triangulation updated: {estimated:F1} (conf: {confidence:F2}), angle diff: {angleDiff:F1}°");

        lastEstimated = estimated;
        if (!missionActive) return;
        triangulated = true;
        missionUI?.ShowTargetLocked(estimated, confidence);
    }

    private Vector3 AzimuthToDirection(float azDeg)
    {
        float rad = azDeg * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)).normalized;
    }

    public void HandlePhoneCalled()
    {
        if (!missionActive) return;
        if (!triangulated)
        {
            missionUI?.ShowStatus("No target locked yet. Triangulate before calling.");
            return;
        }

        missionActive = false;
        antennaA.EndMission(targetPosition, missionActive);
        antennaB.EndMission(targetPosition, missionActive);
        missionUI?.ShowMissionCompleted();
        OnMissionCompleted?.Invoke();
        Debug.Log("Mission completed: call succeeded.");
        
    }
    private void OnDrawGizmos()
    {
        if (!debugShowTarget) return;
        
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawSphere(missionAreaCenter, missionAreaRadius);

        if (missionActive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 1f);
        }
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(lastEstimated, 1f);
        
    }
    private struct AntennaMeasurement
    {
        public Vector3 pos;
        public float az;
    }
}
