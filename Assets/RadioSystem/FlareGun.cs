
using System;
using UnityEngine;

public class FlareGun : MonoBehaviour
{
    [Header("⎯⎯⎯ Объект ракеты ⎯⎯⎯")]
    public GameObject flareGun;
    [Header("⎯⎯⎯ Внутренние данные ⎯⎯⎯")]
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        particle = GetComponent<ParticleSystem>();
        GloboalEventManager.OnFlareGun += StartMission;
    }
    void OnDestroy()
    {
        GloboalEventManager.OnFlareGun -= StartMission;
    }

    private void StartMission()
    {
        particle.Play();
        audioSource.Play();
        Destroy(flareGun, 10f);
    }
}