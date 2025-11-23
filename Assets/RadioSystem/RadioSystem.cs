using System.Collections;
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
    public bool isOn;
    public float offEffectsSmooch = 3f;

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
        isOn = false;
        Off();
    }

    void Update()
    {
        if (isOn)
        {
            finalFrequency = mainFrequency + fineFrequency;
            indicator.currentFrequency = finalFrequency;
            ProcessChannels();
            ProcessNoise();
            ProcessSignals();
        }
    }
    public void Off()
    {
        StopCoroutine(OnEffects());
        StartCoroutine(OffEffects());
    }
    public void On()
    {
        StopCoroutine(OffEffects());
        StartCoroutine(OnEffects());
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

        if (Random.value < crackleChance * (1 - chvolume))
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
    IEnumerator OffEffects()
    {
        isOn = false;
        while (channelSource.volume > 0.001f || noiseSource.volume > 0.001f || fuzzSource.volume > 0.001f || crackleSource.volume > 0.001f)
        {
            channelSource.volume = Mathf.Lerp(channelSource.volume, 0, Time.deltaTime * offEffectsSmooch);
            noiseSource.volume = Mathf.Lerp(noiseSource.volume, 0, Time.deltaTime * offEffectsSmooch);
            fuzzSource.volume = Mathf.Lerp(fuzzSource.volume, 0, Time.deltaTime * offEffectsSmooch);
            crackleSource.volume = Mathf.Lerp(crackleSource.volume, 0, Time.deltaTime * offEffectsSmooch);
            yield return null;
        }
        channelSource.volume = 0;
        noiseSource.volume = 0;
        fuzzSource.volume = 0;
        crackleSource.volume = 0;
        Debug.Log("OffEffects завершен");
    }

    IEnumerator OnEffects()
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
        if (closest == null) 
            yield return null;
        float t = Mathf.Clamp01(bestDist / closest.clearRange);

        float chvolume = (1 - t) * gain;
        float _noiseBase = noiseBase * (1 - chvolume) * gain;
        float _fuzzBase = fuzzBase * (-4f * chvolume * chvolume + 4f * chvolume)* gain;
        float _crackleChance = crackleChance * (1 - chvolume);

        while (noiseSource.volume < _noiseBase - 0.001f || fuzzSource.volume < _fuzzBase - 0.001f || crackleSource.volume < _crackleChance - 0.001f)
        {
            channelSource.volume = Mathf.Lerp(channelSource.volume, chvolume, Time.deltaTime * offEffectsSmooch);
            noiseSource.volume = Mathf.Lerp(noiseSource.volume, _noiseBase, Time.deltaTime * offEffectsSmooch);
            fuzzSource.volume = Mathf.Lerp(fuzzSource.volume, _fuzzBase, Time.deltaTime * offEffectsSmooch);
            crackleSource.volume = Mathf.Lerp(crackleSource.volume, _crackleChance, Time.deltaTime * offEffectsSmooch);
            yield return null;
        }
        channelSource.volume = chvolume;
        noiseSource.volume = _noiseBase;
        fuzzSource.volume = _fuzzBase;
        crackleSource.volume = _crackleChance;
        isOn = true;
        Debug.Log("OnEffects завершен");
    }
}
