using UnityEngine;
public class TunerMain : MonoBehaviour, IFrequencyTarget
{
    public RadioSystem radio;
    public float minFreq = 136f;
    public float maxFreq = 174f;
    public float angle;
    public float step = 0.5f;

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
