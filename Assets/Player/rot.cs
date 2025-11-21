using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rot : MonoBehaviour
{
    public float torque;
    public Rigidbody rb;
    public float rr;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if(transform.rotation != new Quaternion(0, 0, 0, 0))
        {
            rb.AddTorque(new Vector3(transform.rotation.x * -1, transform.rotation.y * -1, transform.rotation.z * -1) * torque * rr);
        }

    }
}
