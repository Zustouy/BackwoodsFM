using UnityEngine;

public class FrequencyIndicator : MonoBehaviour
{
    [Header("Визуализация")]
    public Transform sliderObject;

    [Header("Диапазон частот")]
    public float minFreq = 136f;
    public float maxFreq = 174f;

    [Header("Положение слайдера")]
    public float minX = -0.2f;
    public float maxX =  0.2f;

    [Header("Сглаживание")]
    public float smooth = 10f;

    [Header("Текущая частота")]
    public float currentFrequency;

    void OnEnable()
    {
        GloboalEventManager.OnFrequencyRangeChanged += SetValue;
    }
    public void SetValue(float maxF, float minF)
    {
        minFreq =minF;
        maxFreq =maxF;
    }
    void Update()
    {
        float t = Mathf.InverseLerp(minFreq, maxFreq, currentFrequency);
        float x = Mathf.Lerp(minX, maxX, t);
        Vector3 p = sliderObject.localPosition;
        float newx = Mathf.Lerp(p.x, x, Time.deltaTime * smooth);
        sliderObject.localPosition = new Vector3(newx, p.y, p.z);;
    }
}
