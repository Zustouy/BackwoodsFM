using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOVE : MonoBehaviour
{
    public float speed = 1f;
    Rigidbody rBody;

    private void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 pos = rBody.position;
        rBody.position += Vector3.forward * speed * Time.fixedDeltaTime;

        rBody.MovePosition(pos);
    }
}
