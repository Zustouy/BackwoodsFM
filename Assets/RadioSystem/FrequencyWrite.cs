using TMPro;

using UnityEngine;
public class FrequencyWrite : MonoBehaviour, IAction
{
    [Header("⎯⎯⎯ Внутренние данные ⎯⎯⎯")]
    [SerializeField] private string writeFrequency ;
    [SerializeField] private TextMeshPro wfTMP;
    public  void Interact()
    {
        GloboalEventManager.SendOnFrequencyWrite(float.Parse(writeFrequency));
    }
    public void NumButton(string num)
    {
        if (writeFrequency == null ? true : writeFrequency.Length <= 10)
            wfTMP.text = writeFrequency += num;

    }
    public void DelButton()
    {
        if (writeFrequency != null)
            wfTMP.text = writeFrequency = writeFrequency.Remove(writeFrequency.Length-1, 1);
    }
}