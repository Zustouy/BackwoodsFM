using UnityEngine;

public class FrequencyIndicator : MonoBehaviour
{
    public Transform sliderObject;      // твоя стрелка/ползунок
    public float smooth = 10f;
    public float minFreq = 136f;
    public float maxFreq = 174f;

    public float minX = -0.2f;          // позиция ползунка при minFreq
    public float maxX =  0.2f;          // позиция ползунка при maxFreq

    public float currentFrequency;

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
        float t = Mathf.InverseLerp(minFreq, maxFreq, currentFrequency);
        float x = Mathf.Lerp(minX, maxX, t);
        Vector3 p = sliderObject.localPosition;
        float newx = Mathf.Lerp(p.x, x, Time.deltaTime * smooth);
        sliderObject.localPosition = new Vector3(newx, p.y, p.z);;
    }
}
