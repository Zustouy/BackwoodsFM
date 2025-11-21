using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool OpenDoor = false;
    public GameObject Anim;

    public void Open()
    {

        OpenDoor = !OpenDoor;
        if (OpenDoor)
        {

        }
    }

}
