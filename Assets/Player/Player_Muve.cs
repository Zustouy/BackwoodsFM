using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Player_Muve : MonoBehaviour
{
    [Header("Movement set")]
    private CharacterController _CharacterController;
    private Vector3 _WalkDirection;
    private Vector3 _velocity;
    private Vector3 AirStrfe;
    private Vector3 _AirDirection;
    public float _Crouch = 1.5f;
    public float walkSpeed = 8.0f;
    public float runSpeed = 12.0f;
    public float AirStrfeCoif = 3f;
    public float maximumPlayerSpeed_Run = 12.0f;
    public float maximumPlayerSpeed_Walk = 8f;
    public float OneJump = 1f;
    public float maximumPlayerSpeedStrafe = 100.0f;
    [HideInInspector] public float vInput, hInput;

    [Header("Jump")]
    public float jumpForce = 5.0f;
    public Transform groundChecker;
    public float groundCheckerDist = 0.2f;
    public bool isGrounds = false;

    [Header("Phys")]
    public float Gravity = 9.8f;

    [Header("Other")]
    private float const_high;
    private Vector3 inputForce;
    private float prevY;
    public bool nocip = false;
    private Vector3 _NocipDirection;
    public GameObject Camera;

    [Header("Debud")]
    public bool Debug_ = false;

    // Start is called before the first frame update
    void Start()
    {
        _CharacterController = GetComponent<CharacterController>();
        const_high = _CharacterController.height;
    }

    private void FixedUpdate()
    {

        //Imput Mous
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        isGrounds = _CharacterController.isGrounded;

        if (nocip == false)
        {
            _WalkDirection = transform.right * x + transform.forward * z;

            if (isGrounds)
            {
                Walk(_WalkDirection);
                _AirDirection = _WalkDirection * Time.fixedDeltaTime * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);
            }
            else
            {
                AirStrafe(_AirDirection);
            }
            DoGravity(_CharacterController.isGrounded);
        }
        else
        {
            _NocipDirection = Camera.transform.forward * z + Camera.transform.right * x;
            Nocip(_NocipDirection);
        }
    }

    private void Update()
    {
        print(_velocity);
        //Imput key
        Jump(Input.GetKey(KeyCode.Space) && _CharacterController.isGrounded);
        Crouch(Input.GetKey(KeyCode.LeftControl));
        //Nolcip
        if (Input.GetKeyDown(KeyCode.V) && Debug_ == true)
        {
            nocip = !nocip;
            if (nocip)
            {
                GetComponent<CapsuleCollider>().enabled = false;
            }
            else
            {
                GetComponent<CapsuleCollider>().enabled = true;
                _velocity.y = 0;
                _AirDirection = new Vector3(0, 0, 0);
            }
        }
    }

    //Muve
    private void Walk(Vector3 direction)
    {
        _CharacterController.Move(direction * Time.fixedDeltaTime * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed));
    }
    //Air Strafe
    private void AirStrafe(Vector3 directionAir)
    {
        AirStrfe = (_WalkDirection * Time.fixedDeltaTime * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed))/AirStrfeCoif;
        /*
        if (_velocity.z == 0 || _velocity.x == 0)
        {
            AirStrfe = new Vector3(0, 0, 0);
        }*/

        _CharacterController.Move(directionAir + AirStrfe);
    }

    //Graviti
    private void DoGravity(bool isGrounded)
    {

        if (isGrounded && _velocity.y <= 0)
        {
            _velocity.y = -1f;
        }
        _velocity.y -= Gravity * Time.fixedDeltaTime;
        _CharacterController.Move(_velocity * Time.fixedDeltaTime);
    }

    //Jump
    private void Jump(bool jump)
    {
        if (jump)
        {
            _velocity.y = 0;
            _velocity.y = jumpForce;
        }
    }

    //Crouch
    private void Crouch(bool crouch)
    {
        _CharacterController.height = crouch ? _Crouch : const_high;
    }

    //Noclip Move
    private void Nocip(Vector3 derectionNocip)
    {
        transform.position += derectionNocip * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);
    }
}