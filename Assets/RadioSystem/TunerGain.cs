using UnityEngine;
public class TunerGain : MonoBehaviour
{
    public RadioSystem radio;
    public float angle;

    void Update()
    {
        radio.gain = Mathf.InverseLerp(-170, 170, angle);
    }
}

