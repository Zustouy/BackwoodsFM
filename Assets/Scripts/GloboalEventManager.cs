using System;
using UnityEngine;

public class GloboalEventManager
{
    public static event Action<float, float> OnSignalDetected;

    public static void SendOnSignalDetected(float frequency, float time)
    {
        OnSignalDetected.Invoke(frequency, time);
    }
}
