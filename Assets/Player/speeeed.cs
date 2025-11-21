using Fragsurf.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speeeed : MonoBehaviour
{
    private MoveData _moveData = new MoveData();
    public MoveData moveData { get { return _moveData; } }
    Vector3 _speed;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        _speed = _moveData.velocity;
        print(_speed);
    }
}
