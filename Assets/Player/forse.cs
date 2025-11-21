using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forse : MonoBehaviour
{
    public float fors;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(0,0,fors);
    }
}
