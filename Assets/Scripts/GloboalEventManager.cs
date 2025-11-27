using System;

public class GloboalEventManager
{

    //Управление камерой
    public static event Action<bool> disableCameraMove;

    //Частота
    public static event Action<float> OnFrequencyWrite;

    //Миссия
    public static event Action<float, RadioSignalSOS> OnStartMission;
    public static event Action OnMissionTimeout;
    public static event Action OnMissionCompleted;

    //События в мире
    public static event Action OnFlareGun;
    public static event Action OnPhoneCalled;
    public static event Action OnSosSignalCreate;

    public static void SendDisableCameraMove(bool isinter)
        => disableCameraMove?.Invoke(isinter);

    public static void SendOnFrequencyWrite(float frequency)
        => OnFrequencyWrite?.Invoke(frequency);

    public static void SendOnMissionTimeout()
        => OnMissionTimeout?.Invoke();

    public static void SendOnStartMission(float frequency, RadioSignalSOS sosSignal)
        => OnStartMission?.Invoke(frequency, sosSignal);

    public static void SendOnMissionCompleted()
        => OnMissionCompleted?.Invoke();

    public static void SendOnPhoneCalled()
        => OnPhoneCalled?.Invoke();

    public static void SendOnSosSignalCreate()
        => OnSosSignalCreate?.Invoke();

    public static void SendOnFlareGun()
        => OnFlareGun?.Invoke();
}
