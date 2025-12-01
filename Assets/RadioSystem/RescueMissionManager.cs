using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RescueMissionManager : MonoBehaviour
{
    [Header("⎯⎯⎯ Антенны ⎯⎯⎯")]
    public DirectionalAntenna antennaA;
    public DirectionalAntenna antennaB;

    [Header("⎯⎯⎯ Зона миссии ⎯⎯⎯")]
    public float missionAreaRadius = 200f;
    public Vector3 missionAreaCenter = Vector3.zero;

    [Header("⎯⎯⎯ Префабы и ссылки ⎯⎯⎯")]
    public GameObject prefabScream;
    public GameObject prefabHelper;
    public GameObject prefabFlareGun;
    public Triangulator triangulator;

    [Header("⎯⎯⎯ Настройки миссии ⎯⎯⎯")]
    public float requiredLockAngleDiffDeg = 0.1f;
    public float frequency;
    public float frequencyScaning;
    public TextMeshPro cordOut;


    [Header("⎯⎯⎯ События ⎯⎯⎯")]
    public UnityEvent OnMissionStarted;
    public UnityEvent OnMissionFailed;

    [Header("⎯⎯⎯ Отладка ⎯⎯⎯")]
    public bool debugShowTarget = true;

    [Header("⎯⎯⎯ Внутренние данные ⎯⎯⎯")]
    [SerializeField] private float missionEndTime;
    [SerializeField] private bool hasA = false;
    [SerializeField] private bool hasB = false;
    [SerializeField] private bool missionActive = false;
    [SerializeField] private bool triangulated = false;
    [SerializeField] private AntennaMeasurement measA;
    [SerializeField] private AntennaMeasurement measB;
    [SerializeField] private Vector3 lastEstimated;
    [SerializeField] private Vector3 targetPosition;

    void Start()
    {
        GloboalEventManager.OnStartMission += StartMission;
        GloboalEventManager.OnPhoneCalled += HandlePhoneCalled;
        GloboalEventManager.OnFrequencyWrite += WriteFrequency;
        if (antennaA != null) antennaA.Init(this, AntennaId.AntennaA);
        if (antennaB != null) antennaB.Init(this, AntennaId.AntennaB);
    }

    private void WriteFrequency(float Write)
    {
        frequency = Write;
    }

    public void StartMission(float frequency, RadioSignalSOS missionSignal)
    {
        if (missionActive) return;

        missionActive = true;
        frequencyScaning = frequency;
        targetPosition = GenerateRandomTarget();
        antennaA.StartMission(targetPosition, missionActive, frequency);
        antennaB.StartMission(targetPosition, missionActive, frequency);
        Instantiate(prefabFlareGun, targetPosition, Quaternion.LookRotation(Vector3.up));
        GloboalEventManager.SendOnFlareGun();
        missionEndTime = Time.time + missionSignal.timer;
        hasA = hasB = triangulated = false;
        lastEstimated = Vector3.zero;
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
                OnMissionFailed?.Invoke();
                Debug.Log("Mission failed: timeout");
                yield break;
            }
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
            triangulated = false;
            return;
        }

        bool success = triangulator.TryTriangulate(
            measA.pos, measA.az,
            measB.pos, measB.az,
            out Vector3 estimated, out float confidence);

        if (!success)
        {
            triangulated = false;
            return;
        }
        Debug.Log($"Triangulation updated: {estimated:F1} (conf: {confidence:F2}), angle diff: {angleDiff:F1}°");

        lastEstimated = estimated;
        cordOut.text  = $"X = {estimated.x:000.0} Y = {estimated.z:000.0}";
        if (!missionActive || frequencyScaning != frequency || confidence <= 0.94f) return;
        triangulated = true;
    }

    private Vector3 AzimuthToDirection(float azDeg)
    {
        float rad = azDeg * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)).normalized;
    }

    public void HandlePhoneCalled()
    {
        print("sex");
        if (!triangulated || frequencyScaning != frequency || !missionActive)
        {
            return;
        }

        missionActive = false;
        antennaA.EndMission(targetPosition, missionActive);
        antennaB.EndMission(targetPosition, missionActive);
        GloboalEventManager.SendOnMissionCompleted();
        Instantiate(prefabHelper, targetPosition + Vector3.up, Quaternion.identity);
        Debug.Log("Mission completed: call succeeded.");
        
    }
    private void OnDrawGizmos()
    {
        if (!debugShowTarget) return;
        
        Gizmos.color = new Color(0, 1, 0, 0.5f);
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
