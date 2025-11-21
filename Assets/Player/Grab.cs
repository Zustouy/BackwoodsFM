using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    public bool isGrab;
    public Rigidbody rb_;
    public float Fors_drop;
    public Camera cam_;
    private void FixedUpdate()
    {






        if (isGrab)
        {
            
            rb_.isKinematic = true;
            rb_.MovePosition(transform.position);
        }
        else
        {
            if(rb_ != null)
            {
                rb_.isKinematic = false;
            }
        }
    }

}
