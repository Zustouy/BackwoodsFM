using UnityEngine;
public class TunerGain : MonoBehaviour
{
    [Header("Радиоприёмник")]
    public RadioSystem radio;

    [Header("Положение регулятора усиления")]
    public float angle;
    void Update()
    {
        radio.gain = Mathf.InverseLerp(-170, 170, angle);
    }
}

