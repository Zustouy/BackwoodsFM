using UnityEngine;
public class AntennaTuner : MonoBehaviour
{
    [Header("Антенна")]
    public DirectionalAntenna antenna;
    [Header("Угол поворота")]
    public float angle;
    void Update()
    {
        antenna.RotareAntenna(Mathf.InverseLerp(-1080, 1080, angle));
    }
}

