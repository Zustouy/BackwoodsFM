using UnityEngine;
public class TunerFine : MonoBehaviour
{
    [Header("Радиоприёмник")]
    public RadioSystem radio;

    [Header("Максимальное отклонение (±МГц)")]
    public float fineRange = 0.5f;

    [Header("Шаг настройки (точность)")]
    public float step = 0.005f;

    [Header("Угол поворота ручки")]
    public float angle;    void Update()
    {
        radio.fineFrequency = Mathf.Round(Mathf.Lerp(-fineRange, fineRange, Mathf.InverseLerp(-170, 170, angle)) / step) * step;
    }
}

