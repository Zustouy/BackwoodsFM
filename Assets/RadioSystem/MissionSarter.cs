using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSarter : MonoBehaviour
{
    [Header("⎯⎯⎯ Список SOS-сигналов ⎯⎯⎯")]
    public List<RadioSignalSOS> sosSignals;
    private bool start;
    void Awake()
    {
        GloboalEventManager.OnMissionTimeout += StartNewMission;
        GloboalEventManager.OnMissionCompleted += StartNewMission;
    }
    public void StartNewMission()
    {
        StartCoroutine(TimerToStartNewMission(Random.Range(10 , 30)));
    }
    [ContextMenu("StartMission")]
    public void StartMission()
    {
        RadioSignalSOS sos = sosSignals[Random.Range(0, sosSignals.Count)];
        GloboalEventManager.SendOnStartMission(sos.frequency, sos);
    }
    public void StartFirst()
    {
        CursorManager.HideAndLock();
        if(start)
            return;
        RadioSignalSOS sos = sosSignals[0];
        GloboalEventManager.SendOnStartMission(sos.frequency, sos);
    }
    IEnumerator TimerToStartNewMission(int timer)
    {
        Debug.Log($"До Начала Новой Мисси {timer} c.");
        yield return new WaitForSeconds(timer);
        StartMission();
        Debug.Log("Миссия Начата!");
    }
}