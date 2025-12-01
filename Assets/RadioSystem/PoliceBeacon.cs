using UnityEngine;
using System.Collections;

public class PoliceBeacon : MonoBehaviour
{
    [Header("Light Settings")]
    public Light lamp;                // Сюда лампу
    public float maxIntensity = 3f;   // Максимальная яркость
    public float fadeInTime = 2f;     // Время плавного появления
    public float activeTime = 10f;    // Время активной работы
    public float fadeOutTime = 2f;    // Время затухания
    public float blinkSpeed = 8f;     // Скорость чередования цветов

    [Header("Colors")]
    public Color redColor = Color.red;
    public Color blueColor = Color.blue;

    private void Start()
    {
        if (lamp == null) lamp = GetComponentInChildren<Light>();

        lamp.intensity = 0f;
        StartCoroutine(RunBeacon());
    }

    private IEnumerator RunBeacon()
    {
        yield return StartCoroutine(FadeInBlink());

        yield return StartCoroutine(FullPowerBlink());

        yield return StartCoroutine(FadeOutBlink());

        Destroy(gameObject);
    }
    private IEnumerator FadeInBlink()
    {
        float t = 0f;

        while (t < fadeInTime)
        {
            t += Time.deltaTime;

            lamp.intensity = Mathf.Lerp(0f, maxIntensity, t / fadeInTime);

            lamp.color = (Mathf.FloorToInt(Time.time * blinkSpeed) % 2 == 0)
                ? redColor
                : blueColor;

            yield return null;
        }

        lamp.intensity = maxIntensity;
    }

    private IEnumerator FullPowerBlink()
    {
        float timer = 0f;
        while (timer < activeTime)
        {
            timer += Time.deltaTime;

            lamp.color = (Mathf.FloorToInt(Time.time * blinkSpeed) % 2 == 0)
                ? redColor
                : blueColor;

            lamp.intensity = maxIntensity;

            yield return null;
        }
    }

    private IEnumerator FadeOutBlink()
    {
        float t = 0f;

        while (t < fadeOutTime)
        {
            t += Time.deltaTime;

            lamp.intensity = Mathf.Lerp(maxIntensity, 0f, t / fadeOutTime);

            lamp.color = (Mathf.FloorToInt(Time.time * blinkSpeed) % 2 == 0)
                ? redColor
                : blueColor;

            yield return null;
        }

        lamp.intensity = 0f;
    }
}
