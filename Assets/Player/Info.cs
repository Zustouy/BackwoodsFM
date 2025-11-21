using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info : MonoBehaviour
{
    public Vector3 position;
    public float V;
    public float P;
    public float M;

    private void Start()
    {
        P = M * V;
        V = P / M;
    }

    void OnCollisionStay(Collision collision)
    {
        print(collision.gameObject);
        int laer = LayerMask.NameToLayer("Water");
        if (collision.gameObject.layer == laer)
        {
            float P_ = collision.gameObject.GetComponent<Info>().P;
            float V_ = collision.gameObject.GetComponent<Info>().V;
            float M_ = collision.gameObject.GetComponent<Info>().M;
            V = P / M;
            P = M * V;
            collision.gameObject.GetComponent<Info>().P = M * V;
            P -= M * V;
        }
  
        int laerW = LayerMask.NameToLayer("Door");
        if (collision.gameObject.layer == laerW)
        {
            P = -P;
            V = -V;
        }
    }
    private void Update()
    {
        transform.position += new Vector3(P * M, 0) * Time.deltaTime;
    }

}   