using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Enter : MonoBehaviour
{
    public float Distanc_hit;
    public Camera cam;
    int layerMask = 7;
    public Grab _grab;

    // Update is called once per frame
    private void Start()
    {
        _grab.GetComponent<Grab>();
    }
    void Update()
    {


        if (Input.GetKeyUp(KeyCode.E))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Distanc_hit, layerMask))
            {
                Debug.DrawLine(ray.origin, hit.point);
                if (hit.transform.gameObject.tag == "Door")
                {
                    Door _door = hit.transform.GetComponent<Door>();//Open Door Event
                    if (_door != null)
                    {
                        _door.Open();
                    }
                }
                if (hit.transform.gameObject.tag == "Prop")
                {
                    _grab.isGrab = true;
;
                    //_grab.rb_ = rb;
                    //_grab.Gr(rb);
                    print(hit.transform.gameObject.tag);
                }


            }
            Debug.DrawLine(ray.origin, ray.direction * Distanc_hit);
            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.E))
            {
                if (_grab.isGrab == true)
                {
                    _grab.isGrab = false;
                    Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                    //rb.AddForce(cam_.transform.forward * Fors_drop);
                }
            }
        }
    }
}
