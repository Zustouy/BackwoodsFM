using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Teleporter : MonoBehaviour
{
    public Teleporter Other;
    public CharacterController CharacterController_;
    

    private void OnTriggerStay(Collider other)
    {
        float zPos = transform.worldToLocalMatrix.MultiplyPoint3x4(other.transform.position).z;

        if (zPos < 0) Teleport(other.transform, other.gameObject);
    }

    private void Teleport(Transform obj, GameObject OB)
    {
        // Position
        Vector3 pos = obj.transform.position;
        Vector3 tel = Other.transform.localPosition - transform.localPosition;

        obj.transform.position = new Vector3(pos.x, (pos + tel).y, pos.z);
        /*
        if (OB.CompareTag("Player"))
        {
            CharacterController_.enabled = false;
            obj.transform.position = new Vector3(pos.x, (pos + tel).y, pos.z);
            CharacterController_.enabled = true;
        }
        else
        {
            obj.transform.position = new Vector3(pos.x, (pos + tel).y, pos.z);
        }
        */
    }
}
