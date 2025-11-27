using UnityEngine;
public class TunerMain : MonoBehaviour, IFrequencyTarget
{
    [Header("Радиоприёмник")]
    public RadioSystem radio;

    [Header("Текущий диапазон")]
    public float minFreq = 136f;
    public float maxFreq = 174f;

    [Header("Шаг основной настройки (МГц)")]
    public float step = 0.5f;

    [Header("Положение ручки основной настройки")]
    public float angle;

    void OnEnable()
    {
        RadioSwitch.OnFrequencyRangeChanged += SetValue;
    }
    public void SetValue(float maxF, float minF)
    {
        minFreq =minF;
        maxFreq =maxF;
    }
    void Update()
    {
        radio.mainFrequency = Mathf.Round(Mathf.Lerp(minFreq, maxFreq, Mathf.InverseLerp(-170, 170, angle)) / step) * step;
    }
}
