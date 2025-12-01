using TMPro;

using UnityEngine;
public class FrequencyWrite : MonoBehaviour, IAction
{
    [Header("⎯⎯⎯ Внутренние данные ⎯⎯⎯")]
    [SerializeField] private string writeFrequency ;
    [SerializeField] private TextMeshPro wfTMP;
    [SerializeField] private float parsedFloat;
    public  void Interact()
    {
            if (float.TryParse(writeFrequency, out parsedFloat))
            {
                GloboalEventManager.SendOnFrequencyWrite(parsedFloat);
            }
            else
            {
                Debug.Log("Invalid float format.");
            }
    }
    public void NumButton(string num)
    {
        if (writeFrequency == null ? true : writeFrequency.Length <= 5)
            wfTMP.text = writeFrequency += num;

    }
    public void DelButton()
    {
        if (writeFrequency != null)
            wfTMP.text = writeFrequency = writeFrequency.Remove(writeFrequency.Length-1, 1);
    }
}