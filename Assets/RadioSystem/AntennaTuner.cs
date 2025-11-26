using UnityEngine;
public class AntennaTuner : MonoBehaviour
{
    public DirectionalAntenna antenna;
    public float fineRange = 0.5f;
    public float angle;
    public float step = 0.005f;
    void Update()
    {
        antenna.RotareAntenna(Mathf.Round(Mathf.InverseLerp(-170, 170, angle) / step) * step);
    }
}

