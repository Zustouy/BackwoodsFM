using UnityEngine;
public class AntennaTuner : MonoBehaviour
{
    [Header("Антенна")]
    public DirectionalAntenna antenna;

    [Header("Настройки точной настройки")]
    public float fineRange = 0.5f;
    public float step = 0.005f;

    [Header("Угол поворота")]
    public float angle;
    void Update()
    {
        antenna.RotareAntenna(Mathf.Round(Mathf.InverseLerp(-170, 170, angle) / step) * step);
    }
}

