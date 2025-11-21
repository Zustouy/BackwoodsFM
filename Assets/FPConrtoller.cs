using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    CharacterController cc;

    [Header("Input System")]
    public InputActionAsset inputActions;
    public string actionMapName = "Player";

    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;
    InputAction sprintAction;
    InputAction crouchAction;
    InputAction interactAction;
    InputAction toggleNoclipAction;
    InputAction toggleDebugAction;

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2f;

    public float acceleration = 12f;
    public float deceleration = 12f;

    [Header("Jump")]
    public float jumpHeight = 1.6f;
    public float crouchjumpHeight = 1.6f;
    public float gravity = -24f;

    [Header("Ground Check (CapsuleCast)")]
    public LayerMask groundMask = ~0;
    public float groundCheckRadiusOffset = 0.05f;

    [Header("Crouch")]
    public float standingHeight = 1.8f;
    public float crouchHeight = 1.0f;
    public Vector3 standingCenter = new Vector3(0, -0.9f, 0);
    public Vector3 crouchCenter = new Vector3(0, -0.5f, 0);
    public float crouchSpeedLerp = 10f;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public float interactImpulse = 5f;

    [Header("Noclip")]
    public float noclipSpeed = 10f;
    public float noclipSprintMultiplier = 2f;
    bool noclip = false;

    [Header("Debug")]
    bool debugMode = false;

    // internal
    Vector3 moveVelocity;
    Vector3 targetVelocity;
    Vector3 verticalVelocity;

    bool isGrounded;
    bool isCrouching;

    float currentHeight;
    Vector3 currentCenter;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        currentHeight = standingHeight;
        currentCenter = standingCenter;

        SetupInput();
    }

    void SetupInput()
    {
        var map = inputActions.FindActionMap(actionMapName);

        moveAction = map.FindAction("Move");
        lookAction = map.FindAction("Look");
        jumpAction = map.FindAction("Jump");
        sprintAction = map.FindAction("Sprint");
        crouchAction = map.FindAction("Crouch");
        interactAction = map.FindAction("Interact");
        toggleNoclipAction = map.FindAction("Noclip");
        toggleDebugAction = map.FindAction("Debug");

        map.Enable();
    }

    void Update()
    {
        if (toggleNoclipAction.WasPerformedThisFrame())
        {
            noclip = !noclip;
            cc.enabled = !noclip;
            verticalVelocity = Vector3.zero;
        }

        if (toggleDebugAction.WasPerformedThisFrame())
            debugMode = !debugMode;

        if (noclip) NoclipUpdate();
        else StandardUpdate();
    }

    bool SphereGroundCheck()
    {
        float ccr = cc.radius;
        float ccc = cc.center.y;
        float radius = ccr * 0.95f;
        float groundCheckDist = cc.height / 2f - ccr - ccc + groundCheckRadiusOffset;
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;

        if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, groundCheckDist, groundMask, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(origin, direction * groundCheckDist, Color.green);
            return true;
        }

        Debug.DrawRay(origin, direction * groundCheckDist, Color.red);
        return false;
    }
    void StandardUpdate()
    {
        isGrounded = SphereGroundCheck();

        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // направление камеры
        Vector3 camF = cameraTransform.forward;
        camF.y = 0; camF.Normalize();
        Vector3 camR = cameraTransform.right;
        camR.y = 0; camR.Normalize();

        Vector3 dir = camF * moveInput.y + camR * moveInput.x;

        bool sprint = sprintAction.IsPressed() && !isCrouching;

        float speed = isCrouching ? crouchSpeed : (sprint ? sprintSpeed : walkSpeed);
        targetVelocity = dir.normalized * speed;

        // ИНЕРЦИЯ
        float acc = (targetVelocity.magnitude > moveVelocity.magnitude) ? acceleration : deceleration;
        moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, Time.deltaTime * acc);

        // ВЕРТИКАЛЬНАЯ ФИЗИКА
        if (isGrounded)
        {
            if (verticalVelocity.y < 0) verticalVelocity.y = -2f;

            if (jumpAction.WasPerformedThisFrame())
                if (!isCrouching)
                    verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                else
                    verticalVelocity.y = Mathf.Sqrt(crouchjumpHeight * -2f * gravity);
        }
        else
        {
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        // ПРИСЕДАНИЕ
        HandleCrouch();

        // ДВИЖЕНИЕ
        Vector3 total = moveVelocity + Vector3.up * verticalVelocity.y;
        cc.Move(total * Time.deltaTime);

        // ВЗАИМОДЕЙСТВИЕ
        if (interactAction.WasPerformedThisFrame())
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactDistance))
            {
                var rb = hit.rigidbody;
                if (rb != null && !rb.isKinematic)
                    rb.AddForceAtPosition(cameraTransform.forward * interactImpulse, hit.point, ForceMode.Impulse);
            }
        }
    }

    void HandleCrouch()
    {
        bool wantCrouch = crouchAction.IsPressed();
        
        if (wantCrouch && !isCrouching)
            isCrouching = true;
        else if (!wantCrouch && isCrouching)
        {
            if (CanStand()) isCrouching = false;
        }

        float targetH = isCrouching ? crouchHeight : standingHeight;
        Vector3 targetC = isCrouching ? crouchCenter : standingCenter;

        currentHeight = Mathf.Lerp(currentHeight, targetH, Time.deltaTime * crouchSpeedLerp);
        currentCenter = Vector3.Lerp(currentCenter, targetC, Time.deltaTime * crouchSpeedLerp);

        cc.height = currentHeight;
        cc.center = currentCenter;
    }

    bool CanStand()
    {
        float diff = standingHeight - currentHeight + 0.05f;

        Vector3 p1 = transform.position + Vector3.up * (cc.radius);
        Vector3 p2 = transform.position + Vector3.up * (currentHeight - cc.radius);

        return !Physics.CapsuleCast(
            p1, p2, cc.radius - 0.05f,
            Vector3.up,
            diff,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    void NoclipUpdate()
    {
        Vector2 move = moveAction.ReadValue<Vector2>();
        Vector2 look = lookAction.ReadValue<Vector2>() * 0.1f;

        transform.rotation *= Quaternion.Euler(0, look.x, 0);

        float speed = noclipSpeed * (sprintAction.IsPressed() ? noclipSprintMultiplier : 1f);

        Vector3 dir = cameraTransform.forward * move.y +
                      cameraTransform.right * move.x;

        if (jumpAction.IsPressed()) dir += Vector3.up;
        if (crouchAction.IsPressed()) dir += Vector3.down;

        transform.position += dir.normalized * speed * Time.deltaTime;
    }

    void OnGUI()
    {
        if (!debugMode) return;

        GUI.Box(new Rect(10, 10, 280, 140), "Debug");
        GUI.Label(new Rect(20, 40, 200, 20), $"Grounded: {isGrounded}");
        GUI.Label(new Rect(20, 60, 200, 20), $"Crouching: {isCrouching}");
        GUI.Label(new Rect(20, 80, 200, 20), $"MoveVel: {moveVelocity.magnitude:F2}");
        GUI.Label(new Rect(20, 100, 200, 20), $"VertVel: {verticalVelocity.y:F2}");
        GUI.Label(new Rect(20, 120, 200, 20), $"Noclip: {noclip}");
    }
}
