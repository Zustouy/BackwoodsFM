using System;
using UnityEngine;

public class GloboalEventManager
{
    public static event Action<float, float> OnSignalDetected;
    public static event Action OnMissionTimeout;

    public static void SendOnMissionTimeout()
    {
        OnMissionTimeout?.Invoke();
    }
    public static void SendOnSignalDetected(float frequency, float time)
    {
        OnSignalDetected?.Invoke(frequency, time);
    }
}
