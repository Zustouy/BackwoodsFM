using UnityEngine;
public class TunerFine : MonoBehaviour
{
    public RadioSystem radio;
    public float fineRange = 0.5f;
    public float angle;
    public float step = 0.005f;
    void Update()
    {
        radio.fineFrequency = Mathf.Round(Mathf.Lerp(-fineRange, fineRange, Mathf.InverseLerp(-170, 170, angle)) / step) * step;
    }
}

