using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PhoneInteraction : MonoBehaviour
{
    public TextMeshPro numScreen;
    public AudioSource audioSource;
    [System.Serializable]
    public class PhoneEvent 
    {  
        public string numb;
        public AudioClip call;
        public bool triggersEvent = true;
        public UnityEvent action;
    }

    public List<PhoneEvent> events;
    public Action OnPhoneCalled;
    public string number;
    void Start()
    {
        numScreen.text = null;
    }
    public void Call()
    {
        foreach (var pnum in events)
            if (number == pnum.numb)
            {
                audioSource?.PlayOneShot(pnum.call);
            }
            
        numScreen.text = number = null;
        Debug.Log("Phone: call invoked.");
    }
    public void NumButton(string num)
    {
        if (number == null ? true : number.Length <= 10)
            numScreen.text = number += num;

    }
    public void DelButton()
    {
        if (number != null)
            numScreen.text = number = number.Remove(number.Length-1, 1);

    }
    void OnDrawGizmos()
    {
        string label = $"Call: {number}";
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.1f, label);
        #endif
    }
}
