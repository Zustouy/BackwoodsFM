using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ScreamTheForest : MonoBehaviour
{
    public List<AudioClip> audioClips;
    
    private void Awake()
    {
        GloboalEventManager.OnMissionTimeout += OnScream;
    }
    void OnDestroy()
    {
        GloboalEventManager.OnMissionTimeout -= OnScream;
    }
    private void OnScream()
    {
        if (audioClips != null)
            GetComponent<AudioSource>().PlayOneShot(audioClips[Random.Range(0, audioClips.Count)]);
        Destroy(gameObject, 10f); 
    }

}
