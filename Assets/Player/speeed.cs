using Fragsurf.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speeed : MonoBehaviour
{
    private SurfController _controller = new SurfController();
    float _speed;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        _speed = _controller.speed;
        print(_speed);
    }
}
