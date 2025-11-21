using System.Collections.Generic;
using UnityEngine;

public class RadioSystem : MonoBehaviour
{
    [Header("Tuning")]
    public float mainFrequency;
    public float fineFrequency;
    public float gain = 1.0f;
    public float finalFrequency;
    public FrequencyIndicator indicator;

    [Header("Data")]
    public List<RadioChannel> channels;
    public List<RadioSignalSOS> sosSignals;
    public List<RadioSignalAnomaly> anomalySignals;

    [Header("Audio")]
    public AudioSource channelSource;
    public AudioSource noiseSource;
    public AudioSource fuzzSource;
    public AudioSource crackleSource;

    [Header("Settings")]
    public float noiseBase = 0.3f;
    public float fuzzBase = 0.3f;
    public float crackleChance = 0.05f;

    RadioEventSystem events;

    void Start()
    {
        events = GetComponent<RadioEventSystem>();
    }

    void Update()
    {
        finalFrequency = mainFrequency + fineFrequency;
        indicator.currentFrequency = finalFrequency;
        ProcessChannels();
        ProcessNoise();
        ProcessSignals();
    }

    void ProcessChannels()
    {
        RadioChannel closest = null;
        float bestDist = 999f;

        foreach (var ch in channels)
        {
            float d = Mathf.Abs(finalFrequency - ch.frequency);
            if (d < bestDist)
            {
                bestDist = d;
                closest = ch;
            }
        }

        if (closest == null) return;

        float t = Mathf.Clamp01(bestDist / closest.clearRange);

        if (!channelSource.isPlaying || channelSource.clip != closest.audioClip)
        {
            channelSource.clip = closest.audioClip;
            channelSource.Play();
        }

        channelSource.volume = (1 - t) * gain;
    }

    void ProcessNoise()
    {
        float chvolume = channelSource.volume;
        noiseSource.volume = noiseBase * (1 - chvolume) * gain;
        fuzzSource.volume = fuzzBase * (-4f * chvolume * chvolume + 4f * chvolume)* gain;

        if (Random.value < crackleChance * (1 - channelSource.volume))
            crackleSource.Play();
    }

    void ProcessSignals()
    {
        foreach (var sos in sosSignals)
        {
            float d = Mathf.Abs(finalFrequency - sos.frequency);
            if (d < sos.clearRange)
            {
                events.TriggerEvent(sos.eventName);
            }
        }

        foreach (var a in anomalySignals)
        {
            float d = Mathf.Abs(finalFrequency - a.frequency);
            if (d < a.clearRange)
            {
                events.TriggerEvent(a.eventName);
            }
        }
    }
}
