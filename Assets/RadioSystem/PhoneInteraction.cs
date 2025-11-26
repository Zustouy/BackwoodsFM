using System;
using TMPro;
using UnityEngine;

public class PhoneInteraction : MonoBehaviour
{
    public TextMeshPro numScreen;
    public Action OnPhoneCalled;
    public string number;
    void Start()
    {
        numScreen.text = null;
    }
    public void Call()
    {
        if (number == "911")
        {
            OnPhoneCalled?.Invoke();
            Debug.Log("SEXUALKA CALL.");
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
