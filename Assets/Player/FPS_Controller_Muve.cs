using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class FPS_Controller_Muve : MonoBehaviour

{
    private Rigidbody rb;

    [Header("Movement set")]
    public float walkSpeed = 8.0f;
    public float runSpeed = 12.0f;
    public float AirSpeed = 3f;
    public float maximumPlayerSpeed_Run = 12.0f;
    public float maximumPlayerSpeed_Walk = 8f;
    public float OneJump = 1f;
    public float maximumPlayerSpeedStrafe = 100.0f;
    [HideInInspector] public float vInput, hInput;

    [Header("Jump")]
    public float jumpForce = 500.0f;
    public Transform groundChecker;
    public float groundCheckerDist = 0.2f;
    public bool isGround = false;

    [Header("Phys")]
    public float Gravity = 9.8f;


    private Vector3 inputForce;
    private float prevY;
    public bool nocip = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void FixedUpdate()
    {
        
        print(rb.linearVelocity);

        prevY = rb.linearVelocity.y;
        isGround = (Mathf.Abs(rb.linearVelocity.y - prevY) < .1f) && (Physics.OverlapSphere(groundChecker.position, groundCheckerDist).Length > 1);
        // Input Mous
        vInput = Input.GetAxisRaw("Vertical");
        hInput = Input.GetAxisRaw("Horizontal");
    
        inputForce = (transform.forward * vInput + transform.right * hInput).normalized * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);

        if (isGround)
        {
            rb.linearVelocity = ClampMag(rb.linearVelocity, Input.GetKey(KeyCode.LeftShift) ? maximumPlayerSpeed_Run : maximumPlayerSpeed_Walk);
            if (Input.GetButton("Jump") && isGround == true)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
               OneJump = 0;
            }
            rb.AddForce(inputForce);          
        }
        else
            // Air control
            rb.AddForce(inputForce / AirSpeed);
            rb.linearVelocity = ClampSqrMag(rb.linearVelocity, maximumPlayerSpeedStrafe);

         // Noclip  
        if (Input.GetKey(KeyCode.V))
        {
            nocip = true;
        }


    }
    private static Vector3 ClampMag(Vector3 vec, float maxMag)
    {
        if (vec.sqrMagnitude > maxMag * maxMag)
            vec = vec.normalized * maxMag;
        return vec;
    }
    private static Vector3 ClampSqrMag(Vector3 vec, float sqrMag)
    {
        if (vec.sqrMagnitude > sqrMag)
            vec = vec.normalized * Mathf.Sqrt(sqrMag);
        return vec;
    }

}
