using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RadioSystem : MonoBehaviour
{
    [Header("Точная настройка частоты")]
    public float mainFrequency;
    public float fineFrequency;
    public float finalFrequency;

    [Header("Громкость и усиление")]
    public float gain = 1.0f;

    [Header("UI и индикация")]
    public FrequencyIndicator indicator;
    public TextMeshPro meshPro;

    [Header("Состояние радиостанции")]
    public bool isOn;
    public float offEffectsSmooch = 3f;

    [Header("Радиостанции (каналы)")]
    public List<RadioChannel> channels;

    // [Header("Аномальные сигналы")]
    // public List<RadioSignalAnomaly> anomalySignals;

    [Header("Аудио источники")]
    public AudioSource channelSource;   // чистый канал
    public AudioSource sosSource;       // SOS  сигнал
    public AudioSource noiseSource;     // белый шум
    public AudioSource fuzzSource;      // шипение при расстройке
    public AudioSource crackleSource;   // треск помех

    [Header("Параметры помех")]
    public float channelBase = 0.8f;
    public float sosBase = 0.6f;
    public float noiseBase = 0.3f;
    public float fuzzBase = 0.3f;
    public float crackleChance = 0.05f;

    [Header("Внутренние данные (runtime)")]
    [SerializeField] private float radioAccuracy;
    [SerializeField] private float currenRadioAccuracy;
    [SerializeField] private float currenRadioAccuracySmooch;
    [SerializeField] private Material mat;
    [SerializeField] private Dictionary<RadioChannel, ChannelSwitchData> channelSwitchDatas = new();
    [SerializeField] private RadioSignalSOS sosSignal;    
    private void Awake()
    {
        GloboalEventManager.OnStartMission += StartMission;
        foreach (var channel in channels)
        {
            if (channel.audioClips is not { Count: > 0 }) continue;

            int index = Random.Range(0, channel.audioClips.Count);
            float clipLength = channel.audioClips[index].length;

            float positionInTrack = Random.Range(0f, clipLength);

            channelSwitchDatas[channel] = new ChannelSwitchData
            {
                clipIndex = index,
                startRealtime = Time.realtimeSinceStartup - positionInTrack,
                interruptTime = positionInTrack
            };
        }
        isOn = false;
        mat.SetFloat("_Accuracy",0);
        Off();
    }
    private void StartMission(float arg1, RadioSignalSOS sos)
    {
        sosSignal = sos;
    }

    void Update()
    {
        if (isOn)
        {
            finalFrequency = mainFrequency + fineFrequency;
            meshPro.text = finalFrequency.ToString();
            indicator.currentFrequency = finalFrequency;
            ProcessChannels();
            ProcessNoise();
            ProcessSignals();
            ProcessAccuracy();
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
        float bestDist = Mathf.Infinity;

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
        {
            channelSource.Stop();
            return;
        }

        if (!channelSwitchDatas.TryGetValue(closest, out ChannelSwitchData data))
        {
            data = StartNewTrack(closest, 0);
            channelSwitchDatas[closest] = data;
        }
        else
        {
            float elapsedOffline = Time.realtimeSinceStartup - data.startRealtime;
            AudioClip currentClip = closest.audioClips[data.clipIndex];

            if (elapsedOffline >= currentClip.length)
            {
                data = StartNewTrack(closest, elapsedOffline-currentClip.length);
                channelSwitchDatas[closest] = data;
            }
            else
            {
                data.interruptTime = elapsedOffline;
                channelSwitchDatas[closest] = data;
            }
        }

        AudioClip clipToPlay = closest.audioClips[data.clipIndex];

        if (!channelSource.isPlaying || channelSource.clip != clipToPlay)
        {
            channelSource.clip = clipToPlay;
            channelSource.time = data.interruptTime;
            channelSource.Play();
        }
        float t = radioAccuracy = Mathf.Clamp01(bestDist / closest.clearRange);
        channelSource.volume = (1f - t) * gain;
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
        if (!sosSignal) return;
        RadioSignalSOS closest = null;
        float bestDist = Mathf.Infinity;

        float d = Mathf.Abs(finalFrequency - sosSignal.frequency);
        if (d < bestDist)
        {
            bestDist = d;
            closest = sosSignal;
        }
        
        if (closest == null) return;

        if (!sosSource.isPlaying)
        {
            sosSource.loop = closest.isLoop;
            sosSource.clip = closest.clip;
            sosSource.Play();
        }

        float t = radioAccuracy = Mathf.Clamp01(bestDist / closest.clearRange);

        sosSource.volume =  sosBase * (1 - t) * gain;

        // foreach (var a in anomalySignals)
        // {
        //     float d = Mathf.Abs(finalFrequency - a.frequency);
        //     if (d < a.clearRange)
        //     {
        //         events.TriggerEvent(a.eventName);
        //     }
        // }
    }
    private ChannelSwitchData StartNewTrack(RadioChannel channel, float startTime)
    {
        int index = Random.Range(0, channel.audioClips.Count);
        return new ChannelSwitchData
        {
            clipIndex = index,
            startRealtime = Time.realtimeSinceStartup - startTime,
            interruptTime = 0f
        };
    }
    private void ProcessAccuracy()
    {
        currenRadioAccuracy = Mathf.Lerp(currenRadioAccuracy, 1-radioAccuracy, Time.deltaTime * currenRadioAccuracySmooch);
        mat.SetFloat("_Accuracy",currenRadioAccuracy);
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
    private struct ChannelSwitchData
    {
        public int clipIndex;
        public float startRealtime;
        public float interruptTime;
    }
}
